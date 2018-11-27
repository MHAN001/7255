using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using AdventureGame.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace AdventureAPI.Controllers
{
    
    public class ValuesController : Controller
    {
        const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        public int nextId;
        private string schemaId;
        private string entryId;
        public StoryManager sm = new StoryManager();
        public List<Story> st;
        private IMemoryCache cache;
        Hashtable res ;
        public ValuesController(IMemoryCache cache)
        {
            this.cache = cache;
            this.res = new Hashtable();
        }

        [HttpPost]
        [Route("register")]
        public ActionResult register()
        {
            string usertmp;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                usertmp = sr.ReadToEnd();
            }
            User user = JsonConvert.DeserializeObject<User>(usertmp);
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(user, secret);
            Console.WriteLine(token);
            return new JsonResult(token);
        }

        [HttpPost]
        [Route("Schema")]
        public ActionResult Option1()
        {
            string token = Request.Headers["Authorization"];
            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            string usertmp;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                usertmp = sr.ReadToEnd();
            }
            JSchema schema = JSchema.Parse(usertmp);
            var bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            schemaId = BitConverter.ToString(bytes);
                if (cache.Get(schemaId) != null)
                {
                    return new StatusCodeResult(401);
                }
            cache.Set(schemaId, schema);
            return new JsonResult(schemaId);
        }

        [HttpPost]
        [Route("createRefSchema/{schemaId}")]
        public ActionResult refSchema(string schemaId, string refId)
        {
            //Read Username and bind it to to a score
            string token = Request.Headers["Authorization"];
            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            string newSchema;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                newSchema = sr.ReadToEnd();

            }
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = this.cache.GetType().GetField("_entries", flags).GetValue(this.cache);
            var cacheItems = entries as IDictionary;
            foreach (DictionaryEntry item in cacheItems)
            {
                if (!res.ContainsKey(item.Key))
                {
                    res.Add(item.Key, cacheItems[item.Key]);
                }
                {
                    res[item.Key] = this.cache.Get(item.Key);
                }
            }
            if (res[refId] == null)
            {
                return new StatusCodeResult(400);
            }
            string schemaRef = Convert.ToString(res[refId]);
            JSchemaPreloadedResolver resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri("http://localhost:7000/api/entry/"+refId), schemaRef);
            JSchema schema = JSchema.Parse(newSchema, resolver);
            cache.Set(schemaId, schema);
            return new StatusCodeResult(200);
        }

        [HttpPost]
        [Route("Entry")]
        public ActionResult Option2()
        {
            string token = Request.Headers["Authorization"];
            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = this.cache.GetType().GetField("_entries", flags).GetValue(this.cache);
            var cacheItems = entries as IDictionary;

            if (cacheItems == null) return new NotFoundObjectResult("There is no schema in the database!");
            foreach (DictionaryEntry item in cacheItems)
            {
                if (!res.ContainsKey(item.Key))
                {
                    res.Add(item.Key, cacheItems[item.Key]);
                }
                {
                    res[item.Key] = this.cache.Get(item.Key);
                }
            }
            string info;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                info = sr.ReadToEnd();

            }
            JObject json = JObject.Parse(info);
            string responseEtag = "this is Etag";
            foreach (dynamic js in res.Values)
            {

                try
                {
                    json.Validate((JSchema)js);
                    Boolean valid = json.IsValid((JSchema)js);
                    if (valid)
                    {
                        var bytes = new byte[16];
                        using (var rng = new RNGCryptoServiceProvider())
                        {
                            rng.GetBytes(bytes);
                        }
                        entryId = BitConverter.ToString(bytes);
                        cache.Set(entryId, json);
                        Response.Headers.Add("ETag", responseEtag);
                        return new JsonResult(entryId);
                    }
                }
                catch(Exception e)
                {
                    continue;
                }
            }
            return new NotFoundObjectResult("No such schema currently available!");
        }
        

        [HttpGet]
        [Route("validate")]
        public ActionResult validate()
        {
            string token = Request.Headers["Authorization"];
            if (validateUser(token))
            {
                return new AcceptedResult();
            }

            return new StatusCodeResult(401);
        }    

        [HttpGet]
        [Route("entry/{entryid}")]
        public ActionResult getInfo(string entryid)
        {
            //Read Username and bind it to to a score
            string token = Request.Headers["Authorization"];
            string etag = Request.Headers["etag"];

            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = this.cache.GetType().GetField("_entries", flags).GetValue(this.cache);
            var cacheItems = entries as IDictionary;

            if (cacheItems == null) return null;
            foreach (DictionaryEntry item in cacheItems)
            {
                if (!res.ContainsKey(item.Key))
                {
                    res.Add(item.Key, cacheItems[item.Key]);
                }
                {
                    res[item.Key] = this.cache.Get(item.Key);
                }
            }
            if (!res.ContainsKey(entryid))
            {
                return new StatusCodeResult(404);
            }
            return new JsonResult(res[entryid]);
        }

        [HttpGet]
        [Route("entry")]
        public ActionResult getRecords()
        {
            string token = Request.Headers["Authorization"];
            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = this.cache.GetType().GetField("_entries", flags).GetValue(this.cache);
            var cacheItems = entries as IDictionary;

            if (cacheItems == null) return null;
            foreach (DictionaryEntry item in cacheItems)
            {

                if (!res.ContainsKey(item.Key))
                {
                    res.Add(item.Key, cacheItems[item.Key]);
                }
                {
                    res[item.Key] = this.cache.Get(item.Key);
                }
            }
            //res.Cast<DictionaryEntry>().ToList();
            return new JsonResult(res);
        }


        [HttpPut]
        [Route("{entryType}/{entryId}")]
        public ActionResult update(string entryType, string entryId)
        {
            //TODO: avoid entry/schemaId to update schema
            string token = Request.Headers["Authorization"];
            if (entryType.Equals("schema"))
            {
                if (!validateUser(token))
                {
                    return new StatusCodeResult(401);
                }
            }
            string usertmp;
            using (StreamReader sr = new StreamReader(Request.Body))
            {
                usertmp = sr.ReadToEnd();
            }
            JSchema schema = JSchema.Parse(usertmp);
            if (cache.Get(entryId) != null)
            {
                cache.Set(entryId, schema);
                return new OkObjectResult("Update Successfully");
            }
            return new NotFoundObjectResult("Cannot find entry");
        }


        [HttpDelete]
        [Route("{entrytype}/{id}")]
        public ActionResult deleteEntry(string entrytype, string id)
        {
            string token = Request.Headers["Authorization"];
            if (!validateUser(token))
            {
                return new StatusCodeResult(401);
            }
            if (this.cache.Get(id) == null)
            {
                return new NotFoundObjectResult("Delete failed cause there is no such entry");
            }
            if (entrytype.Equals("schema"))
            {
                return new StatusCodeResult(401);
            }
            this.cache.Remove(id);
            return new OkObjectResult("Delete successfully!");
        }


        private Boolean validateUser(string token)
        {
            if(token == null)
            {
                return false;
            }
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);
                return true;
            }
            catch (TokenExpiredException)
            {
                return false;
            }
            catch (SignatureVerificationException)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }

    public class User
    {
        public string name { get; set; }
        public string pwd { get; set; }
    }
}
