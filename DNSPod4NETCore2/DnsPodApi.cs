using DNSPod4NETCore2.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DNSPod4NETCore2
{
    public class DnsPodApi : IDnsPod
    {
        private readonly IDnsPodClient client;
        public DnsPodApi(IDnsPodClient client)
        {
            this.client = client;
        }

        #region 创建记录

        /// <summary>
        /// 创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <returns>记录ID</returns>
        public string Create(int domainId, string subDomain, string recordValue)
        {
            return Create(domainId, subDomain, recordValue, "A", "默认");
        }

        /// <summary>
        /// 创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <param name="recordType">记录类型，通过API记录类型获得，大写英文，比如：A</param>
        /// <param name="recordLine">记录线路，通过API记录线路获得，中文，比如：默认</param>
        /// <returns>记录ID</returns>
        public string Create(int domainId, string subDomain, string recordValue, string recordType, string recordLine)
        {
            string recordId = null;
            object p = new
            {
                domain_id = domainId,
                sub_domain = subDomain,
                record_type = recordType,
                record_line = recordLine,
                value = recordValue
            };
            recordId = Create(p);
            return recordId;
        }

        /// <summary>
        /// 创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <param name="recordType">记录类型，通过API记录类型获得，大写英文，比如：A</param>
        /// <param name="recordLineId">记录线路ID，通过API记录线路获得，中文，比如：默认</param>
        /// <returns>记录ID</returns>
        public string Create(string domainName, string subDomain, string recordValue, string recordType = "A", string recordLineId = "默认")
        {
            string recordId = null;
            object p = new
            {
                domain = domainName,
                sub_domain = subDomain,
                record_type = recordType,
                record_line_id = recordLineId,
                value = recordValue
            };
            recordId = Create(p);
            return recordId;
        }

        /// <summary>
        /// 创建记录
        /// domain_id 域名ID, 必选
        /// sub_domain 主机记录, 如 www, 默认@，可选
        /// record_type 记录类型，通过API记录类型获得，大写英文，比如：A, 必选
        /// record_line 记录线路，通过API记录线路获得，中文，比如：默认, 必选
        /// value 记录值, 如 IP:200.200.200.200, CNAME: cname.dnspod.com., MX: mail.dnspod.com., 必选
        /// mx {1-20} MX优先级, 当记录类型是 MX 时有效，范围1-20, MX记录必选
        /// ttl {1-604800} TTL，范围1-604800，不同等级域名最小值不同, 可选
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public string Create(object paramObject)
        {
            dynamic result = client.PostApiRequest("Record.Create", paramObject);
            return Convert.ToInt32(result.status.code) == 1 ? result.record.id : null;
        }

        #endregion

        #region 域名列表

        /// <summary>
        /// 域名列表
        /// </summary>
        /// <returns></returns>
        public DnsPodDomainList DomainList()
        {
            return JsonConvert.DeserializeObject<DnsPodDomainList>(client.PostApiRequest("Domain.List", null).ToString());
        }

        #endregion

        #region 设置域名状态

        /// <summary>
        /// 设置域名状态
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public bool SetDomainStatus(object paramObject)
        {
            dynamic result = client.PostApiRequest("Domain.Status", paramObject);
            return Convert.ToInt32(result.status.code) == 1;
        }

        /// <summary>
        /// 设置域名状态
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="enabled">域名状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public bool SetDomainStatus(int domainId, bool enabled)
        {
            return SetDomainStatus(new {
                domain_id = domainId,
                status = enabled ? "enable" : "disable"
            });
        }

        /// <summary>
        /// 设置域名状态
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="enabled">域名状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public bool SetDomainStatus(string domainName, bool enabled)
        {
            return SetDomainStatus(new {
                domain = domainName,
                status = enabled ? "enable" : "disable"
            });
        }

        /// <summary>
        /// 启用域名
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public bool ResumeDomain(int domainId) => SetDomainStatus(domainId, true);

        /// <summary>
        /// 启用域名
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns></returns>
        public bool ResumeDomain(string domainName) => SetDomainStatus(domainName, true);

        /// <summary>
        /// 暂停域名
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public bool PauseDomain(int domainId) => SetDomainStatus(domainId, false);

        /// <summary>
        /// 暂停域名
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns></returns>
        public bool PauseDomain(string domainName) => SetDomainStatus(domainName, false);

        #endregion

        #region 记录列表

        /// <summary>
        /// 记录列表
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public dynamic RecordList(int domainId)
        {
            return JsonConvert.DeserializeObject<DnsPodRecordList>(RecordList(new { domain_id = domainId }).ToString());
        }

        /// <summary>
        /// 记录列表
        /// </summary>
        /// <param name="domainId">域名</param>
        /// <returns></returns>
        public DnsPodRecordList RecordList(string domain)
        {
            return JsonConvert.DeserializeObject<DnsPodRecordList>(RecordList(new { domain }).ToString());
        }

        public dynamic RecordList(object paramObject)
        {
            return client.PostApiRequest("Record.List", paramObject);
        }

        #endregion

        #region 修改记录

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="domainName">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="value">记录值</param>
        /// <param name="subDomain">主机记录（二级域名）</param>
        /// <param name="recordType">记录类型（默认为“A”）</param>
        /// <param name="recordLineId">记录线路（默认为“默认”）</param>
        /// <returns>操作是否成功</returns>
        public bool Modify(int domainId, string recordId, string value, string subDomain, string recordType = "A", string recordLineId = "0")
        {
            return Modify(new
            {
                domain_id = domainId,
                record_id = recordId,
                value,
                sub_domain = subDomain,
                record_type = recordType,
                record_line_id = recordLineId,
            });
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="value">记录值</param>
        /// <param name="subDomain">主机记录（二级域名）</param>
        /// <param name="recordType">记录类型（默认为“A”）</param>
        /// <param name="recordLineId">记录线路（默认为“默认”）</param>
        /// <returns>操作是否成功</returns>
        public bool Modify(string domainName, string recordId, string value, string subDomain, string recordType = "A", string recordLineId = "0")
        {
            return Modify(new
            {
                domain = domainName,
                record_id = recordId,
                value,
                sub_domain = subDomain,
                record_type = recordType,
                record_line_id = recordLineId,
            });
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns>操作是否成功</returns>
        public bool Modify(object paramObject)
        {
            dynamic result = client.PostApiRequest("Record.Modify", paramObject);
            return Convert.ToInt32(result.status.code) == 1;
        }

        #endregion

        #region 删除记录

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public bool Remove(object paramObject)
        {
            dynamic result = client.PostApiRequest("Record.Remove", paramObject);
            return Convert.ToInt32(result.status.code) == 1;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool Remove(int domainId, string recordId)
        {
            return Remove(new { domain_id = domainId, record_id = recordId });
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool Remove(string domainName, string recordId)
        {
            return Remove(new { domain = domainName, record_id = recordId });
        }

        #endregion

        #region 设置记录状态

        /// <summary>
        /// 设置记录状态
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public bool SetRecordStatus(object paramObject)
        {
            dynamic result = client.PostApiRequest("Record.Status", paramObject);
            return Convert.ToInt32(result.status.code) == 1;
        }

        /// <summary>
        /// 设置记录状态
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="enabled">记录状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public bool SetRecordStatus(int domainId, string recordId, bool enabled)
        {
            return SetRecordStatus(new {
                domain_id = domainId,
                record_id = recordId,
                status = enabled ? "enable" : "disable"
            });
        }

        /// <summary>
        /// 设置记录状态
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="enabled">记录状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public bool SetRecordStatus(string domainName, string recordId, bool enabled)
        {
            return SetRecordStatus(new
            {
                domain = domainName,
                record_id = recordId,
                status = enabled ? "enable" : "disable"
            });
        }

        /// <summary>
        /// 启用记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool ResumeRecord(int domainId, string recordId) => SetRecordStatus(domainId, recordId, true);

        /// <summary>
        /// 启用记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool ResumeRecord(string domainName, string recordId) => SetRecordStatus(domainName, recordId, true);

        /// <summary>
        /// 暂停记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool PauseRecord(int domainId, string recordId) => SetRecordStatus(domainId, recordId, false);

        /// <summary>
        /// 暂停记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public bool PauseRecord(string domainName, string recordId) => SetRecordStatus(domainName, recordId, false);

        #endregion
        
        #region 设置DDNS

        private bool Ddns(object paramObject)
        {
            dynamic result = client.PostApiRequest("Record.Ddns", paramObject);
            return Convert.ToInt32(result.status.code) == 1;
        }

        public bool Ddns(int domainId, string recordId, string subDomain, string value)
        {
            return Ddns(domainId, recordId, subDomain, "默认", value);
        }

        public bool Ddns(int domainId, string recordId, string subDomain, string recordLine, string value)
        {
            return Ddns(new
            {
                domain_id = domainId,
                record_id = recordId,
                sub_domain = subDomain,
                record_line = recordLine,
                value
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="recordId"></param>
        /// <param name="value"></param>
        /// <param name="record_line"></param>
        /// <param name="subDomain"></param>
        /// <returns></returns>
        public bool Ddns(string domainName, string recordId, string value, string subDomain = "", string recordLine = "默认")
        {
            if (!string.IsNullOrWhiteSpace(subDomain))
            {
                return Ddns(new
                {
                    domain = domainName,
                    record_id = recordId,
                    sub_domain = subDomain,
                    record_line = recordLine,
                    value
                });
            }
            else
            {
                return Ddns(new
                {
                    domain = domainName,
                    record_id = recordId,
                    record_line = recordLine,
                    value
                });
            }
        }


        #endregion

        #region 设置记录备注

        public bool Remark(int domainId, string recordId, string subDomainremark)
        {
            dynamic result = client.PostApiRequest("Record.Remark", new { });
            return Convert.ToInt32(result.status.code) == 1;
        }

        #endregion

        #region 获取记录信息

        public dynamic Info(int domainId, string recordId)
        {
            return Info(new
            {
                domain_id = domainId,
                record_id = recordId
            });
        }

        /// <summary>
        /// 获取记录信息
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public dynamic Info(object paramObject) => client.PostApiRequest("Record.Info", paramObject);
        #endregion

        #region 异步创建记录

        /// <summary>
        /// 异步创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <returns>记录ID</returns>
        public Task<string> CreateAsync(int domainId, string subDomain, string recordValue)
            => Task.Run(() => Create(domainId, subDomain, recordValue));

        /// <summary>
        /// 异步创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <param name="recordType">记录类型，通过API记录类型获得，大写英文，比如：A</param>
        /// <param name="recordLine">记录线路，通过API记录线路获得，中文，比如：默认</param>
        /// <returns>记录ID</returns>
        public Task<string> CreateAsync(int domainId, string subDomain, string recordValue, string recordType, string recordLine)
            => Task.Run(() => Create(domainId, subDomain, recordValue, recordType, recordLine));

        /// <summary>
        /// 异步创建记录,默认记录类型为A
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="subDomain">二级域名名称</param>
        /// <param name="recordValue">记录值</param>
        /// <param name="recordType">记录类型，通过API记录类型获得，大写英文，比如：A</param>
        /// <param name="recordLineId">记录线路ID，通过API记录线路获得，中文，比如：默认</param>
        /// <returns>记录ID</returns>
        public Task<string> CreateAsync(string domainName, string subDomain, string recordValue, string recordType = "A", string recordLineId = "默认")
            => Task.Run(() => Create(domainName, subDomain, recordValue, recordType, recordLineId));

        /// <summary>
        /// 异步创建记录
        /// domain_id 域名ID, 必选
        /// sub_domain 主机记录, 如 www, 默认@，可选
        /// record_type 记录类型，通过API记录类型获得，大写英文，比如：A, 必选
        /// record_line 记录线路，通过API记录线路获得，中文，比如：默认, 必选
        /// value 记录值, 如 IP:200.200.200.200, CNAME: cname.dnspod.com., MX: mail.dnspod.com., 必选
        /// mx {1-20} MX优先级, 当记录类型是 MX 时有效，范围1-20, MX记录必选
        /// ttl {1-604800} TTL，范围1-604800，不同等级域名最小值不同, 可选
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public Task<string> CreateAsync(object paramObject)
            => Task.Run(() => Create(paramObject));

        #endregion

        #region 异步获取域名列表

        /// <summary>
        /// 异步获取域名列表
        /// </summary>
        /// <returns></returns>
        public Task<DnsPodDomainList> DomainListAsync()
            => Task.Run(() => DomainList());

        #endregion

        #region 异步设置域名状态

        /// <summary>
        /// 异步设置域名状态
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public Task<bool> SetDomainStatusAsync(object paramObject)
            => Task.Run(() => SetDomainStatus(paramObject));

        /// <summary>
        /// 异步设置域名状态
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="enabled">域名状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public Task<bool> SetDomainStatusAsync(int domainId, bool enabled)
            => Task.Run(() => SetDomainStatus(domainId, enabled));

        /// <summary>
        /// 异步设置域名状态
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="enabled">域名状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public Task<bool> SetDomainStatusAsync(string domainName, bool enabled)
            => Task.Run(() => SetDomainStatus(domainName, enabled));

        /// <summary>
        /// 启用域名
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public Task<bool> ResumeDomainAsync(int domainId)
            => Task.Run(() => ResumeDomain(domainId));

        /// <summary>
        /// 启用域名
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns></returns>
        public Task<bool> ResumeDomainAsync(string domainName)
            => Task.Run(() => ResumeDomain(domainName));

        /// <summary>
        /// 暂停域名
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public Task<bool> PauseDomainAsync(int domainId)
            => Task.Run(() => PauseDomain(domainId));

        /// <summary>
        /// 暂停域名
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns></returns>
        public Task<bool> PauseDomainAsync(string domainName)
            => Task.Run(() => PauseDomain(domainName));

        #endregion

        #region 异步获取记录列表

        /// <summary>
        /// 异步获取记录列表
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <returns></returns>
        public Task<dynamic> RecordListAsync(int domainId)
            => Task.Run(() => RecordList(domainId));

        /// <summary>
        /// 异步获取记录列表
        /// </summary>
        /// <param name="domainId">域名</param>
        /// <returns></returns>
        public Task<DnsPodRecordList> RecordListAsync(string domain)
            => Task.Run(() => RecordList(domain));

        public Task<dynamic> RecordListAsync(object paramObject)
            => Task.Run(() => RecordList(paramObject));

        #endregion

        #region 异步修改记录

        /// <summary>
        /// 异步修改记录
        /// </summary>
        /// <param name="domainName">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="value">记录值</param>
        /// <param name="subDomain">主机记录（二级域名）</param>
        /// <param name="recordType">记录类型（默认为“A”）</param>
        /// <param name="recordLineId">记录线路（默认为“默认”）</param>
        /// <returns>操作是否成功</returns>
        public Task<bool> ModifyAsync(int domainId, string recordId, string value, string subDomain, string recordType = "A", string recordLineId = "0")
            => Task.Run(() => Modify(domainId, recordId, value, subDomain, recordType, recordLineId));

        /// <summary>
        /// 异步修改记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="value">记录值</param>
        /// <param name="subDomain">主机记录（二级域名）</param>
        /// <param name="recordType">记录类型（默认为“A”）</param>
        /// <param name="recordLineId">记录线路（默认为“默认”）</param>
        /// <returns>操作是否成功</returns>
        public Task<bool> ModifyAsync(string domainName, string recordId, string value, string subDomain, string recordType = "A", string recordLineId = "0")
            => Task.Run(() => Modify(domainName, recordId, value, subDomain, recordType, recordLineId));

        /// <summary>
        /// 异步修改记录
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns>操作是否成功</returns>
        public Task<bool> ModifyAsync(object paramObject)
            => Task.Run(() => Modify(paramObject));

        #endregion

        #region 异步删除记录

        /// <summary>
        /// 异步删除记录
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(object paramObject)
            => Task.Run(() => Remove(paramObject));

        /// <summary>
        /// 异步删除记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(int domainId, string recordId)
            => Task.Run(() => Remove(domainId, recordId));

        /// <summary>
        /// 异步删除记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(string domainName, string recordId)
            => Task.Run(() => Remove(domainName, recordId));

        private Task<bool> DdnsAsync(object paramObject)
            => Task.Run(() => Ddns(paramObject));

        public Task<bool> DdnsAsync(int domainId, string recordId, string subDomain, string value)
            => Task.Run(() => Ddns(domainId, recordId, subDomain, value));

        public Task<bool> DdnsAsync(int domainId, string recordId, string subDomain, string recordLine, string value)
            => Task.Run(() => Ddns(domainId, recordId, subDomain, recordLine, value));

        #endregion

        #region 异步设置记录状态

        /// <summary>
        /// 异步设置记录状态
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public Task<bool> SetRecordStatusAsync(object paramObject)
            => Task.Run(() => SetRecordStatus(paramObject));

        /// <summary>
        /// 异步设置记录状态
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="enabled">记录状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public Task<bool> SetRecordStatusAsync(int domainId, string recordId, bool enabled)
            => Task.Run(() => SetRecordStatus(domainId, recordId, enabled));

        /// <summary>
        /// 异步设置记录状态
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <param name="enabled">记录状态（true:启用; false:暂停）</param>
        /// <returns></returns>
        public Task<bool> SetRecordStatusAsync(string domainName, string recordId, bool enabled)
            => Task.Run(() => SetRecordStatus(domainName, recordId, enabled));

        /// <summary>
        /// 异步启用记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> ResumeRecordAsync(int domainId, string recordId)
            => Task.Run(() => ResumeRecord(domainId, recordId));

        /// <summary>
        /// 异步启用记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> ResumeRecordAsync(string domainName, string recordId)
            => Task.Run(() => ResumeRecord(domainName, recordId));

        /// <summary>
        /// 异步暂停记录
        /// </summary>
        /// <param name="domainId">域名ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> PauseRecordAsync(int domainId, string recordId)
            => Task.Run(() => PauseRecord(domainId, recordId));

        /// <summary>
        /// 异步暂停记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public Task<bool> PauseRecordAsync(string domainName, string recordId)
            => Task.Run(() => PauseRecord(domainName, recordId));

        #endregion

        #region 异步设置DDNS

        /// <summary>
        /// 异步设置DDNS
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="recordId"></param>
        /// <param name="value"></param>
        /// <param name="record_line"></param>
        /// <param name="subDomain"></param>
        /// <returns></returns>
        public Task<bool> DdnsAsync(string domainName, string recordId, string value, string subDomain = "", string recordLine = "默认")
            => Task.Run(() => DdnsAsync(domainName, recordId, value, subDomain, recordLine));

        #endregion

        #region 异步设置记录备注

        public Task<bool> RemarkAsync(int domainId, string recordId, string subDomainremark)
            => Task.Run(() => Remark(domainId, recordId, subDomainremark));

        #endregion

        #region 异步获取记录信息

        public Task<dynamic> InfoAsync(int domainId, string recordId)
            => Task.Run(() => Info(domainId, recordId));
        
        /// <summary>
        /// 异步获取记录信息
        /// </summary>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public Task<dynamic> InfoAsync(object paramObject)
            => Task.Run(() => Info(paramObject));
        #endregion
    }
}
