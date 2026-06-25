using EcosCLM.Application.Exceptions;
using EcosCLM.Application.Interfaces;
using EcosCLM.Application.Services;
using EcosCLM.Application.ViewModels;
using EcosCLM.Domain.Entities.Base;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcosCLM.Application.Extensions
{
    public static class AuditLogsExtension
    {
        public static AuditLogsViewModel GetById(this IAuditLogsRepository repository, Guid id)
        {
            var entity = repository?.GetAll()
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (entity == null)
                throw new NotFoundException(nameof(AuditLogs), id);

            return repository.ToViewModel(entity);
        }

        public static List<AuditLogsViewModel> GetAllWithPage(this IAuditLogsRepository repository, int page = 0, int offset = 0, string filter = null, string oderBy = null, string orderDirection = null, Guid? Customer = null)
        {
            var query = repository?.GetAll();

            query = query.Where(x => x.IdCustumer == Customer);

            if (!string.IsNullOrEmpty(oderBy))
            {
                switch (oderBy)
                {
                    case "date":
                        query = orderDirection == "desc" ? query.OrderByDescending(i => i.Date) : query.OrderBy(i => i.Date);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.Date);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var search = JsonConvert.DeserializeObject<AuditLogsViewModel>(filter);

                if (!string.IsNullOrEmpty(search.User))
                    query = query.Where(x => x.User.Contains(search.User));

                if (search.IdCustumer != Guid.Empty)
                    query = query.Where(x => x.IdCustumer == search.IdCustumer);

                if (search.SearchStartDate != null)
                    query = query.Where(x => x.Date > search.SearchStartDate);

                if (search.SearchEndDate != null)
                    query = query.Where(x => x.Date < search.SearchEndDate);
            }

            if (page > 0)
                query = query.Take(page);

            if (offset > 0)
                query = query.Skip(offset);

            return repository.ToListViewModel(query.ToList());
        }

        public static AuditLogsViewModel Create(this IAuditLogsRepository repository, AuditLogs entity)
        {
            var query = repository.Add(entity);
            return repository.ToViewModel(query);
        }

        public static AuditLogsViewModel Create(this IAuditLogsRepository repository, AuditLogs entity, ISyslogService _syslogService, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            string NormalizeIp(IPAddress? ip)
            {
                if (ip == null)
                    return "Unknown";

                if (ip.IsIPv4MappedToIPv6)
                    return ip.MapToIPv4().ToString();

                if (ip.ToString() == "::1")
                    return "127.0.0.1";

                return ip.ToString() ?? "Unknown";
            }

            entity.SourceIp = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            entity.DestinationIp = httpContext?.Connection.LocalIpAddress?.ToString() ?? "Unknown";

            StringBuilder hashData = new StringBuilder();
            hashData.Append(entity.Date.ToString("o"));
            hashData.Append(entity.User);
            hashData.Append(entity.IdCustumer);
            hashData.Append(entity.LogType);
            hashData.Append(entity.Log);
            hashData.Append(entity.SourceIp);
            hashData.Append(entity.DestinationIp);
            hashData.Append(hashData.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(hashData.ToString());
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                entity.Hash = sb.ToString();
            }

            var query = repository.Add(entity);

            try
            {
                // Enviar log ao Syslog
                _syslogService.Initialize(entity.IdCustumer);
                _syslogService.SendLog("Ecos Dashboard", entity, SyslogSeverity.Information);

            }
            catch (Exception ex)
            {

            }

            return repository.ToViewModel(query);
        }
    }
}
