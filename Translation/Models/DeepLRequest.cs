using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LoreBridge.Translation.Models;

public sealed class DeepLRequest
{
    public DeepLRequest(long id, string sentence, string sourceLanguage, string targetLanguage)
    {
        Id = id;
        Jsonrpc = "2.0";
        Method = "LMT_handle_jobs";

        List<Job> jobs = new()
        {
            new Job(sentence)
        };

        Params = new Parameters(jobs, new Lang(sourceLanguage, targetLanguage));
    }

    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("jsonrpc")] public string Jsonrpc { get; set; }

    [JsonPropertyName("method")] public string Method { get; set; }

    [JsonPropertyName("params")] public Parameters Params { get; set; }

    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this).Replace("hod\":\"",
            (Id + 3) % 13 == 0L || (Id + 5) % 29 == 0L ? "hod\" : \"" : "hod\": \"");
    }

    public sealed class Parameters
    {
        public Parameters(List<Job> jobs, Lang lang)
        {
            Priority = 1L;
            // CommonJobParams = new CommonJobParams();

            var num = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            var num2 = 2L;
            var num3 = num;
            Timestamp = num3 + (num2 - num3 % num2);

            Jobs = jobs;
            Lang = lang;
        }

        [JsonPropertyName("commonJobParams")] public CommonJobParams CommonJobParams { get; set; }

        [JsonPropertyName("priority")] public long Priority { get; set; }

        [JsonPropertyName("timestamp")] public long Timestamp { get; set; }

        [JsonPropertyName("jobs")] public List<Job> Jobs { get; set; }

        [JsonPropertyName("lang")] public Lang Lang { get; set; }
    }

    public sealed class CommonJobParams
    {
        [JsonPropertyName("formality")] public object Formality { get; set; }

        [JsonPropertyName("regionalVariant")] public string RegionalVariant { get; set; }
    }


    public sealed class Job
    {
        public Job(string sentence)
        {
            Kind = "default";
            NewLineFollows = false;
            PreferredNumBeams = 1;
            RawEnSentence = sentence;
            RawEnContextBefore = new List<string>();
            RawEnContextAfter = new List<string>();
        }

        [JsonPropertyName("kind")] public string Kind { get; set; }

        [JsonPropertyName("raw_en_sentence")] public string RawEnSentence { get; set; }

        [JsonPropertyName("raw_en_context_before")]
        public List<string> RawEnContextBefore { get; set; }

        [JsonPropertyName("raw_en_context_after")]
        public List<string> RawEnContextAfter { get; set; }

        [JsonPropertyName("preferred_num_beams")]
        public long PreferredNumBeams { get; set; }

        [JsonIgnore] public bool NewLineFollows { get; set; }
    }

    public sealed class Lang
    {
        public Lang(string sourceLanguage, string targetLanguage)
        {
            SourceLangComputed = sourceLanguage;
            TargetLang = targetLanguage;
            UserPreferredLangs = new[] { sourceLanguage, targetLanguage };
        }

        [JsonPropertyName("source_lang_computed")]
        public string SourceLangComputed { get; set; }

        [JsonPropertyName("user_preferred_langs")]
        public string[] UserPreferredLangs { get; set; }

        [JsonPropertyName("target_lang")] public string TargetLang { get; set; }
    }
}