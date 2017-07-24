using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace multiLangBE.Controllers {

    public class ValuesController : Controller {
        // GET api/values
        [HttpGet]
        [Route ("api/values")]
        public IEnumerable<string> Get () {
            // // public string Get () {
            IEnumerable<string> returnValue = Directory.GetFiles(@"D:\multiLang\src\app\lang", "*.json")
                .Select(Path.GetFileNameWithoutExtension);
            return returnValue;
        }

        // GET api/values/en
        [HttpGet]
        [Route ("api/values/{lang}")]
        public string Get (string lang) {
           string json = System.IO.File.ReadAllText (@"D:\multiLang\src\app\lang\"+lang+".json");
           return json;
        }

        // GET api/values/
        // Write Zipfile
        // [HttpGet ()]
        // [Route ("api/export")]
        // public string Export () {
        //     try {
        //         string startPath = @"D:\multiLang\src\app\lang";
        //         string zipPath = @"C:\Users\puwanart\Desktop\lang-" + DateTime.Now.ToString("yyyy_dd_M-HH_mm_ss") + ".zip";
        //         ZipFile.CreateFromDirectory(startPath, zipPath);
        //         return "0";
        //     } catch (Exception ex) {
        //         return ex.ToString();
        //     }
            
        // }

        [HttpGet]
        [Route ("api/compare/{lang1}/{lang2}")]
        public string Compare(string lang1, string lang2) {
            try {
                string json1 = System.IO.File.ReadAllText (@"D:\multiLang\src\app\lang\"+lang1+".json");
                string json2 = System.IO.File.ReadAllText (@"D:\multiLang\src\app\lang\"+lang2+".json");
                JObject o1 = JsonConvert.DeserializeObject<JObject>(json1);
                JObject o2 = JsonConvert.DeserializeObject<JObject>(json2);
                o1.Merge(o2, new JsonMergeSettings {
                    // union array values together to avoid duplicates
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                Sort(o1);
                return o1.ToString();
            } catch (Exception ex) {
                return ex.ToString();
            }
        }

        // POST api/values/en
        [HttpPost]
        [Route ("api/values/{lang}")]
        public string Post (string lang, [FromBody] JObject value) {
            try {
                string json = JsonConvert.SerializeObject (value, Formatting.Indented);
                System.IO.File.WriteAllText (@"D:\multiLang\src\app\lang\"+lang+".json", json);
                return "0";
            } catch (Exception ex) {
                return ex.ToString();
            }
            
        }

        [HttpGet]
        [Route("api/download")]
        public ActionResult Download()
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd");
            string filename = "export["+datetime+"].zip";

            string zipPath = @"D:\multiLang\src\app\log\" + filename;
            string myPath = @"D:\multiLang\src\app\lang\";
            ZipFile.CreateFromDirectory(myPath, zipPath);
            
            // ------------------------------------------------------------------------------------
            byte[] filedata = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath);
            string contentType = "application/zip";
            
            Response.Headers.Add("content-disposition", "attachment; filename=" + filename);

            return File(filedata, contentType);
        }

        public void Sort(JObject jObj)
        {
            var props = jObj.Properties().ToList();
            foreach (var prop in props)
            {
                prop.Remove();
            }

            foreach (var prop in props.OrderBy(p=>p.Name))
            {
                jObj.Add(prop);
                if(prop.Value is JObject)
                    Sort((JObject)prop.Value);
            }
        }

        

    }

}