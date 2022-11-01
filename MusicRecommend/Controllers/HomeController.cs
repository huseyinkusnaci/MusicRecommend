using MusicRecommend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicRecommend.Controllers
{
    public class HomeController : Controller
    {
        public static List<TrainingDataModel> trainingDataList = new List<TrainingDataModel>();
    
        public ActionResult Index()
        {
            trainingDataList = FillTraningData();

            Random random = new Random();
            int UserID = random.Next(1, 10);
            UserSelectModel userSelectModel = new UserSelectModel();
            //Training datas are grouped and sent to top 5 artist view 
            userSelectModel.FirstFiveArtist = trainingDataList.GroupBy(d => d.ArtistId).Select(d => d.Key).OrderBy(d => d).Take(5).ToArray();

            //Except for the top 5 artists, we got the results of the votes of the other users, excluding the votes of the current user.
            List<AnalysModel> analys = trainingDataList.Where(d =>
            d.UserId != UserID && !userSelectModel.FirstFiveArtist.Contains(d.ArtistId)
            ).GroupBy(d=>d.ArtistId).Select(d =>
            new AnalysModel() {
                ArtistId = d.Key, 
                UserCount = d.Count(c => c.Liked == "Y"),
                Users = d.Where(c => c.Liked == "Y"
                ).Select(c => c.UserId).ToArray() }).OrderByDescending(d=>d.UserCount).Take(5).ToList();

            userSelectModel.UserId = UserID;
          
            userSelectModel.SuggestArtistList = analys;
             
           

            return View(userSelectModel);

        }
        public ActionResult PostUserResponse(List<TrainingDataModel> model)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://f1func-001.azurewebsites.net/api/TrainingDataUpdate?code=2EiVjdBauREP4kyVOXLUDLYCJRjJ1Ud/e6LLqL8YFBg0JP9kTU4XTw==");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            //trainingDataList = FillTrnData();
             
            return View();
        }
       
        

        public List<TrainingDataModel> FillTraningData()
        {
            List<TrainingDataModel> freshtrainingDataList = new List<TrainingDataModel>();
            var request = WebRequest.Create("https://s3.amazonaws.com/f1fs001-aws/TrainingData.csv");
            request.Timeout = 7000;
            string responseCsv;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if ((response != null) && (response.StatusCode == HttpStatusCode.OK))
                    {
                       
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                            responseCsv = sr.ReadToEnd();

                    
                    }
                    else
                    {
                        return new List<TrainingDataModel>();
                    }
                }
            }
            catch (Exception ex)
            {

                return freshtrainingDataList;
            }
            string[] responseCsvLines = responseCsv.Split('\n').ToArray();
            for (int i = 1; i <= responseCsvLines.Count()-1; i++)
            {
                try
                {
                    string[] dividedLine = responseCsvLines[i].Replace("\r", "").Split(',').ToArray();
                    int UserId = 0;
                    int ArtistId = 0;
                    bool isUserCorect = int.TryParse(dividedLine[0], out UserId);
                    bool isArtistCorect = int.TryParse(dividedLine[1], out ArtistId);
                    if (UserId != 0 && ArtistId != 0)
                    {
                        TrainingDataModel data = new TrainingDataModel()
                        {

                            UserId = UserId,
                            ArtistId = ArtistId,
                            Liked = dividedLine[2]

                        };
                        freshtrainingDataList.Add(data);
                    }
                }
                catch (Exception ex)
                {

                   
                }
                
             
               
            }

 
            return freshtrainingDataList;
              
        }
      


    }
}