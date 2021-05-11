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
        static string[] sep;
        /// <summary>
        /// Get the text from local data || 
        /// (Database name).(Grid name).(Record ID).(Column ID language index )
        /// </summary>
        /// <returns>translated text with the current language</returns>
        public static string GetStringData(this string path)
        {
            name = path.Split('.');
            try
            {
                Record record = Project.singleton.databases.Find(x => x.databaseName == name[0])
                .grids.Find(x => x.nameGrid == name[1])
                .records.Find(x => x.recordID == name[2]);

                foreach(var column in record.columns)
                {
                    sep = column.columnID.Split('_');
                    if(sep[0] == Project.singleton.targetLanguage.ToString() && name[3] == sep[1] )
                    {
                        return column.text;
                    }
                }
                
                
            }
            catch(Exception e)
            {
                Debug.Log("Path does not exist. Please make sure you entered the correct path format, and added data");
            }

            return "";
        }




    }

}
