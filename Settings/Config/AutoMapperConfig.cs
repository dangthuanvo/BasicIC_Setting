using AutoMapper;
using BasicIC_Setting.Models.Main.M02;
using Ninject;
using Repository.EF;
using Settings.Models.Main.M02;
using System;

namespace BasicIC_Setting.Config
{
    public class AutoMapperConfig
    {
        static readonly IKernel kernel = null;
        public static MapperConfiguration MapperConfiguration()
        {
            return new MapperConfiguration(config =>
            {
                config.ConstructServicesUsing(t => kernel.Get(t));
                config.AddProfile(new MappingProfile());
            });
        }
    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DateTime?, Nullable<TimeSpan>>().ConvertUsing(new DateTimeToTimeSpanConverter());
            CreateMap<Nullable<TimeSpan>, Nullable<DateTime>>().ConvertUsing(new TimeSpanToDateTimeConverter());

            // Add as many of these lines as you need to map your objects
            // M02
            {

                // Email
                CreateMap<EmailSettingModel, M02_BasicEntities>().ReverseMap();
                CreateMap<DefaultCommonSettingModel, M02_DefaultCommonSetting>().ReverseMap();
                CreateMap<CommonSettingModel, M02_CommonSetting>().ReverseMap();
                //CreateMap<EmailTemplateModel, M02_EmailTemplate>().ReverseMap();

            }
        }

        public class DateTimeToTimeSpanConverter : ITypeConverter<DateTime?, Nullable<System.TimeSpan>>
        {
            public Nullable<System.TimeSpan> Convert(DateTime? source, Nullable<System.TimeSpan> destination, ResolutionContext context)
            {
                if (source != null)
                    return source.Value.TimeOfDay;
                else
                    return null;
            }
        }

        public class TimeSpanToDateTimeConverter : ITypeConverter<Nullable<System.TimeSpan>, Nullable<DateTime>>
        {
            public DateTime? Convert(Nullable<System.TimeSpan> source, DateTime? destination, ResolutionContext context)
            {
                if (source != null)
                    return new DateTime() + source.Value;
                else
                    return null;
            }
        }
    }
}