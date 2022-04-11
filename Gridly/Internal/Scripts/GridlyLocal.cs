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
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="recordID"></param>
        /// <returns>translated text with the current language</returns>
        public static string GetStringData(string grid, string recordID)
        {
            try
            {
                /*
                Debug.Log("gridId: " + grid + " recordID: " + recordID);
                Debug.Log(Project.singleton.grids[0].nameGrid);
                Debug.Log(Project.singleton.grids[0].records.Count);
                */
                Record record = Project.singleton
                .grids.Find(x => x.nameGrid == grid)
                .records.Find(x => x.recordID == recordID);

                

                foreach (var column in record.columns)
                {
                    try
                    {
                        //Debug.Log(Project.singleton.targetLanguage.languagesSuport.ToString());
                        if (column.columnID == Project.singleton.targetLanguage.languagesSuport.ToString())
                        {
                            return column.text;
                        }
                    }
                    catch // try to return other language if cant found target language
                    {
                        Debug.Log("cant found: " + recordID + " | code:" + Project.singleton.targetLanguage.languagesSuport.ToString());
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
