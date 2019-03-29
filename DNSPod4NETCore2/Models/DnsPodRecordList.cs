using Newtonsoft.Json;
using System.Collections.Generic;

namespace DNSPod4NETCore2.Models
{
    public class DnsPodRecordList
    {
        [JsonProperty("status")]
        public Status Status { get; set; }
        [JsonProperty("domain")]
        public Domain Domain { get; set; }
        [JsonProperty("info")]
        public Info Info { get; set; }
        [JsonProperty("records")]
        public List<Record> Records { get; set; }
    }

    public class DnsPodDomainList
    {
        [JsonProperty("status")]
        public Status Status { get; set; }
        [JsonProperty("info")]
        public Info Info { get; set; }
        [JsonProperty("domains")]
        public List<Domain> Domains { get; set; }
    }

    public class Status
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }

    public class Domain
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("punycode")]
        public string Punycode { get; set; }
        [JsonProperty("grade")]
        public string Grade { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("ext_status")]
        public string ExtStatus { get; set; }
        [JsonProperty("ttl")]
        public int TTL { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }

    public class Info
    {
        [JsonProperty("sub_domains")]
        public string SubDomains { get; set; }
        [JsonProperty("record_total")]
        public string RecordTotal { get; set; }
        [JsonProperty("records_num")]
        public string RecordsNum { get; set; }
    }

    public class Record
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("line")]
        public string Line { get; set; }
        [JsonProperty("line_id")]
        public string LineId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("ttl")]
        public string TTL { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("weight")]
        public object Weight { get; set; }
        [JsonProperty("mx")]
        public string Mx { get; set; }
        [JsonProperty("enabled")]
        public string Enabled { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("monitor_status")]
        public string MonitorStatus { get; set; }
        [JsonProperty("remark")]
        public string Remark { get; set; }
        [JsonProperty("updated_on")]
        public string UpdatedOn { get; set; }
        [JsonProperty("use_aqb")]
        public string UseAqb { get; set; }

        public override string ToString()
        {
            return string.Format("[{3}] {0} ({1}): {2}", Name, Type, Value, "1" == Enabled ? "E" : "D");
        }
    }
}
