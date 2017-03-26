using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WordNetProxy.Dtos;
using System.Net.Http;
using Newtonsoft.Json;

namespace WordNetProxy.Controllers
{
    
    public class ValuesController : Controller
    {
        [HttpPost]
        [Route("api/values/StartTast")]
        public async Task<ActionResult> StartTask([FromBody]PostStartTaskDto dto)
        {
            HttpClient client = new HttpClient();
            var url = $"http://ws.clarin-pl.eu/nlprest2/base/startTask/";
            var jsonDto = JsonConvert.SerializeObject(dto);

            StringContent content = new StringContent(jsonDto, System.Text.Encoding.UTF8,
                    "application/json");

            var response = await client.PostAsync(url, content).Result.Content.ReadAsStringAsync();

            return Ok(response);
        }

        [HttpGet]
        [Route("api/values/DownloadWsd/")]
        public async Task<ActionResult> DownloadWsd([FromQuery] String fileId)
        {
            HttpClient client = new HttpClient();
            var url = $"http://ws.clarin-pl.eu/nlprest2/base/download{fileId}";

            var response = await client.GetAsync(url).Result.Content.ReadAsStringAsync();

            return Ok(response);
        }

        [HttpGet]
        [Route("api/values/GetStatus/{id}")]
        public async Task<ActionResult> GetStatus([FromRoute] String id)
        {
            HttpClient client = new HttpClient();
            var url = $"http://ws.clarin-pl.eu/nlprest2/base/getStatus/{id}?_=1490514033966";

            var response = await client.GetAsync(url).Result.Content.ReadAsStringAsync();

            return Ok(response);
        }

        [HttpPost]
        [Route("api/values/GetSynsets")]
        public async Task<ActionResult> GetSynsets([FromBody] SynsetQueryDto dto)
        {
            HttpClient client = new HttpClient();
            var url = $"http://ws.clarin-pl.eu/lexrest/lex";

            var tasks = new List<Task<HttpResponseMessage>>();
            foreach(var synset in dto.SynsetIds)
            {
                var request = new { id = synset, task = "synset", tool = "plwordnet" };
                var stringContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8,
                    "application/json");
                var task = client.PostAsync(url, stringContent);
                tasks.Add(task);
            }

            var responses = await Task.WhenAll<HttpResponseMessage>(tasks);
            var stringResponses = new List<dynamic>();
            foreach (var item in responses)
            {
                var stringResp = await item.Content.ReadAsStringAsync();
                stringResponses.Add(JsonConvert.DeserializeObject<dynamic>(stringResp));
            }
            return Ok(stringResponses);
        }
    }
}
