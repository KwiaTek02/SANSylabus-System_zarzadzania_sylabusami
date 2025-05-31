using AutoMapper;
using SylabusAPI.Models;    // przestrzeń nazw wygenerowanych modeli EF
using SylabusAPI.DTOs;      // przestrzeń nazw Twoich DTO
using SylabusAPI.Data;

namespace SylabusAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Tutaj definiujesz mapowanie
            CreateMap<przedmioty, PrzedmiotDto>()
                .ForMember(
                    dest => dest.SumaGodzinCalosciowe,
                    opt => opt.MapFrom(src => src.suma_godzin_calosciowe)
                );
            // (opcjonalnie odwrotne, jeśli będziesz mapować DTO na encję)
            CreateMap<PrzedmiotDto, przedmioty>();


            CreateMap<siatka_przedmiotow, SiatkaPrzedmiotowDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.PrzedmiotId,
                    opt => opt.MapFrom(src => src.przedmiot_id))
                .ForMember(dest => dest.Typ,
                    opt => opt.MapFrom(src => src.typ))
                .ForMember(dest => dest.Wyklad,
                    opt => opt.MapFrom(src => src.wyklad))
                .ForMember(dest => dest.Cwiczenia,
                    opt => opt.MapFrom(src => src.cwiczenia))
                .ForMember(dest => dest.Konwersatorium,
                    opt => opt.MapFrom(src => src.konwersatorium))
                .ForMember(dest => dest.Laboratorium,
                    opt => opt.MapFrom(src => src.laboratorium))
                .ForMember(dest => dest.Warsztaty,
                    opt => opt.MapFrom(src => src.warsztaty))
                .ForMember(dest => dest.Projekt,
                    opt => opt.MapFrom(src => src.projekt))
                .ForMember(dest => dest.Seminarium,
                    opt => opt.MapFrom(src => src.seminarium))
                .ForMember(dest => dest.Konsultacje,
                    opt => opt.MapFrom(src => src.konsultacje))
                .ForMember(dest => dest.Egzaminy,
                    opt => opt.MapFrom(src => src.egzaminy))
                .ForMember(dest => dest.SumaGodzin,
                    opt => opt.MapFrom(src => src.sumagodzin));
            CreateMap<SiatkaPrzedmiotowDto, siatka_przedmiotow>();
        }
    }
}