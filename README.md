# gridly-unity-plugin

- 1.0.1 - 29/5/2021
  + fixed posting of special alphabets automatically converted to '?'

-1.0.2 - 2/6/2021
  + fixed an error occurs when switching tabs in some cases
  + fixed not delete record on gridly 
 
-1.0.3 - 9/6/2021
  + remove some unnecessary feature
  
-1.0.4 - 9/7/2021
 + remove database system
 + custom view


# GRIDLY PLUGIN
​
# How to install
  Download the GridlyPlugin.unitypackage file and drag it into the project tab in Unity.
  
# How to find the plugin documentation
  You will find the plugin documentation in this folder Gridly/doc or via this [link](https://docs.google.com/document/d/1aoUgzhqS1pTJHyTO2WBIrNB5pckqqPLSdJtWHUeBpYo/edit?fbclid=IwAR01L6Phs8an71qw0-gu17p4mr5Z0Y5l73O7Cy9X6h_twOGWNAxMCEVOXpk)

​
​
​
# 1.  What this plugin can do
​
This Plugin can help you get data from Gridly to Unity and turn it into local data, using the path you can get information based on the target language.
​
​
​
# 2. Setup data on Gridly
​
To be able to use this plugin with Gridly, there is some initial setup you will have to do within Gridly so that the plugin can refer to the correct columns in Gridly. First you need to specify a **columnID**. This you have to do for each of the columns containing language texts. To do this you simply open the grid in Gridly that you want to use with the plugin. Then for each of the language columns you select the option **Format column**.
![](https://lh6.googleusercontent.com/QKptHetzMrMUIXJhvg765xGsQ5LOjeF3MvSe7cLCprjIRfZKp2Fpn2Q5omZy2IwDq0Bi2vr4ctrz7Lf0IZqsrh0mJ4cEN75qQZ-pJrTKVs83vdgYKIcAnKOzTOjyw51ENMPNYqv5)
​
Then you can change the **columnID** to the specific four character **language code** that applies to the text in the column:
​
![](https://lh6.googleusercontent.com/1brUzLB_YPK-a6zjwJFq-gLPd7mVYekgcOnQvkvBpoQ3pSKQhINUswhpjYdIoNl_YU4w6ZpW1AOb7_tvEghlRzJzFGcipcO80Bw4Ku4uK0NzzKXpOeNleSLN8iWGxZPTGRXzkhwh)
​
## 2.1 Setup data on Gridly
First, you need to open the Gridly setting window.
Go to: **window-> Gridly -> Setting**
![](https://lh4.googleusercontent.com/0hbAuOkG9r-StePuCcV65rGIKg1keFdUSL7tU9wIhfI7fk382jVJtZPjmP7KUgiQXO30Lv2eMpCxgipSxPQ3AQBt-1D-w0HCm_ml2adFkL9J37f0_jgA9yHHUpOLVOkKxKyPeTvS)
​
Enter your API at below “**Enter your API key**” then enter your **project ID**. If you don't know what your Project ID is you can find it in the search bar  to see all your project IDs at.
​
Please click save when you have done then click “**Get data from Gridly**”.
![](https://lh4.googleusercontent.com/VRMuJ70z-CAJyhdKP4fCDhsXOGePEKyOrzEsUifKGNyfG97u4swy3ZKA0FP59mClEv4OTwu8V0TkJrP7QPFvItl0rZWDyMyNZkZKqWFDmNeCjpv5_Hr4qEovPip5Z509EprRbpiT)
​
# 3 How to use
## 3.1 **String Editor**
 - Select **Tool->Gridly->String** to open term editor
 - Update button : update new data to Gridly
 - Delete Button : delete the record on Gridly and Unity
 - Rename Button : Rename the key on Gridly and Unity
​
![](https://lh5.googleusercontent.com/60bzjHL_gnUFkvAe4nUV7sTCr6Kd5-z-DTvmXZiXWjDC2iIsKTuErTyHoL7h0W4fwAN-s9ew6Z29MeOhpF0G9WWNg0NwXja7m6Otf28W76zLEGcjzrRJmV_mm5GqGdVJJF2mT_aM)
​
## 3.2 **Sync Schedule**
To install the sync schedule from Gridly to Unity select **Tool->Gridly->Setup->Setting** then select the Sync Schedule tab and then select the schedule you want to update.
![](https://lh4.googleusercontent.com/LMypLJni5-kG5I8yyNuai7rjoY2KnCISgNoZa2qjTzgAXttvjWn4LENrj_QkADpMIxa4Fh8w-5k11w_GvLuIlQTr-pMxtEatGlhzzCqZvD3at5L-5yIIkDUvgupUNaOsC4zljhuN)
​
**Note**: Syncing from Gridly via Unity is for testing purposes only.
​
## 3.3 **Translate Text**
​
 - Use **Project.singleton.SetChosenLanguageCode(string langCode)** to set your target language
 - Use **GridlyLocal.GetStringData(database, grid, key)** to get your text
 - Or use this component
![](https://lh3.googleusercontent.com/i1CqT4TlN9QN1gqcAgPWt57LTbhv5gNrjGzQPDqPEwVNCYcxAoOgr9MyDedzHhQcv3zSDp71dOwYNJ9MwyrVV0O_Ou2xYNqlvARS5yMnEsAvUgheqFecBg_bkhIzUUdUJlNuC7FF)
 - Or you can search for your key with a string: <`<DatabaseName>.<GridName>.<RecordID>.`
 ![](https://lh5.googleusercontent.com/-Zdro4aYJ5w4iuPOHw8dajj4KY1UkaBg5717dTXgdzQbtdNLBXZaFjKEn0tyl1ACKA4qcuwsFQX95ALmv1WBuMbtBn1XsL2oO7udrLPC87q9SUgGDLSa0TYHaVyocr98y_WfDCqY)
​
