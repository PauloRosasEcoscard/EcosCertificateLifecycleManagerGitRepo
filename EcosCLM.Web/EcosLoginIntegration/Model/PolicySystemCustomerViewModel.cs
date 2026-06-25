using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EcosCLM.Web.EcosLoginIntegration.Model
{
    public class PolicySystemCustomerViewModel
    {
        public Guid IdCustomer { get; set; }
        public string TxTitle { get; set; }
        public string TxRepresentative { get; set; }
        public string TxPhone { get; set; }
        public string TxUrl { get; set; }
        public int NuStatus { get; set; }
        public DateTime DtCreated { get; set; }
    }
}