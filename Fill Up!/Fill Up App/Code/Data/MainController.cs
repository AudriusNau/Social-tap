﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Fill_Up_App.Code.Activities;

namespace Fill_Up_App.Code.Data
{
    public static class MainController
    {
        private const string Url = "http://10.0.2.2:5000";
        public static Dictionary<string, BarData> BarsData;

        private const int PostRequestTimeout = 3000;
        private const int GetRequestTimeout = 5000;

        public static async void AddBarReview(BarReview barReview)
        {

            var client = new RestClient(Url);
            var request = new RestRequest($"/api/values/AddBarReview/{barReview.BarName}/{barReview.RatingOfBar}", Method.POST)
            {
                Timeout = PostRequestTimeout
            };

            var cancellationTokenSource = new CancellationTokenSource();

            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            var requestStatus = response.ResponseStatus;
            if (requestStatus.Equals(ResponseStatus.None) || requestStatus.Equals(ResponseStatus.TimedOut)
                || requestStatus.Equals(ResponseStatus.Error))
                MainActivity.ReportAddBarReviewState(false);
            else
            {
                var processingStatus = JsonConvert.DeserializeObject<bool>(response.Content);
                MainActivity.ReportAddBarReviewState(requestStatus.Equals(ResponseStatus.Completed) && processingStatus);
            }
        }

        /// <summary>
        /// Fetchs the bars data.
        /// </summary>
        /// <returns>The bars data.</returns>
        public static async Task<bool> FetchBarsData()
        {
            BarsData = new Dictionary<string, BarData>();
            var client = new RestClient(Url);
            var request = new RestRequest("/api/values/GetBarData/", Method.GET) { Timeout = GetRequestTimeout };
            var cancellationTokenSource = new CancellationTokenSource();

            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

            var requestStatus = response.ResponseStatus;

            if (requestStatus.Equals(ResponseStatus.None) || requestStatus.Equals(ResponseStatus.TimedOut)
                || requestStatus.Equals(ResponseStatus.Error))
            {
                return false;
            }

            var content = response.Content;
            BarsData = JsonConvert.DeserializeObject<Dictionary<string, BarData>>(content);
            return requestStatus.Equals(ResponseStatus.Completed);
        }
    }
}