using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gridly.Internal;

namespace Gridly
{
    public enum Languages
    {
        enUS,
        arSA,
        frFR,
        zhCN,
        zhTW,
        deDE,
        itIT,
        jaJP,
        koKR,
        plPL,
        ptBR,
        ruRU,
        esES,
        esMX,
        caES,
        bnBD,
        bgBG,
        zhHK,
        afZA,
        arAE,
        arBH,
        arDZ,
        arEG,
        arIQ,
        arJO,
        arKW,
        arLB,
        arLY,
        arMA,
        arOM,
        arQA,
        arSY,
        arTN,
        arYE,
        azAZ,
        beBY,
        bsBA,
        csCZ,
        cyGB,
        daDK,
        deAT,
        deCH,
        deLI,
        deLU,
        dvMV,
        elGR,
        enAU,
        enBZ,
        enCA,
        enGB,
        enIE,
        enJM,
        enNZ,
        enPH,
        enTT,
        enZA,
        enZW,
        esAR,
        esBO,
        esCL,
        esCO,
        esCR,
        esDO,
        esEC,
        esGT,
        esHN,
        esNI,
        esPA,
        esPE,
        esPR,
        esPY,
        esSV,
        esUY,
        esVE,
        etEE,
        euES,
        faIR,
        fiFI,
        foFO,
        frBE,
        frCA,
        frCH,
        frLU,
        frMC,
        glES,
        guIN,
        heIL,
        hiIN,
        hrBA,
        hrHR,
        huHU,
        hyAM,
        idID,
        isIS,
        itCH,
        kaGE,
        kkKZ,
        knIN,
        kyKG,
        ltLT,
        lvLV,
        miNZ,
        mkMK,
        mnMN,
        mrIN,
        msBN,
        msMY,
        mtMT,
        nbNO,
        nlBE,
        nlNL,
        nnNO,
        nsZA,
        paIN,
        psAR,
        ptPT,
        quBO,
        quEC,
        quPE,
        roRO,
        saIN,
        seFI,
        seNO,
        seSE,
        skSK,
        siSI,
        sqAL,
        srBA,
        svFI,
        svSE,
        swKE,
        taIN,
        teIN,
        thTH,
        tlPH,
        tnZA,
        trTR,
        ttRU,
        ukUA,
        urPK,
        uzUZ,
        viVN,
        xhZA,
        zhMO,
        zhSG,
        zuZA,
    }

    public static class GridlyLocal
    {

        static string[] name;
        /// <summary>
        /// Get the text from local data || 
        /// DatabaseName.GridName.RecordID)
        /// </summary>
        /// <returns>translated text with the current language</returns>
        public static string GetStringData(this string path)
        {
            name = path.Split('.');
            try
            {
                return GetStingData(name[0], name[1], name[2]);
            }
            catch
            {
                Debug.LogError("the path should follow databaseName.GridName.RecordID");
                return "";
            }
        }


        public static string GetStingData(string database, string grid, string recordID)
        {
            try
            {
                Record record = Project.singleton.databases.Find(x => x.databaseName == database)
                .grids.Find(x => x.nameGrid == grid)
                .records.Find(x => x.recordID == recordID);

                

                foreach (var column in record.columns)
                {
                    try
                    {
                        if (column.columnID == Project.singleton.targetLanguage.languagesSuport.ToString())
                        {
                            return column.text;
                        }
                    }
                    catch // try to return other language if cant found target language
                    {
                        foreach(var i in Project.singleton.langSupports)
                        {
                            if (column.columnID == i.languagesSuport.ToString())
                            {
                                return column.text;
                            }
                        }
                    }
                }

            }
            catch
            {
                Debug.Log("Path does not exist. Please make sure you entered the correct path format, and added data");
            }

            return "";
        }

    }

}
