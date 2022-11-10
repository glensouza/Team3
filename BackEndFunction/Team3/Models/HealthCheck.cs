using System;
using System.Text.Json.Serialization;

namespace Team3.Models
{
    public class HealthCheck
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }
        [JsonPropertyName("submissionDateTime")]
        public DateTime SubmissionDateTime { get; set; }
        [JsonPropertyName("healthStatus")]
        public string HealthStatus { get; set; }
        [JsonPropertyName("symptoms")]
        public string[] Symptoms { get; set; }
    }
}
