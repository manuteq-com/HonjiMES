using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HonjiMES.Models;

namespace HonjiMES
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap< WorkOrderDetail, WorkOrderDetailData>();
        }
    }
}
