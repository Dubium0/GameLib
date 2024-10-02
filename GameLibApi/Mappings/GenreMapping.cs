using System;
using GameLibApi.Dtos.GenreDtos;
using GameLibApi.Entities;


namespace GameLibApi.Mappings;
public static class GenreMapping
{
    public static Genre ToEntity(this UpdateGenreDto genreDto, int id){
            return new Genre(){
            Id = id,
            Name = genreDto.Name
        };
    }

    public static Genre ToEntity(this CreateGenreDto genreDto){
        return new Genre() {
            Name = genreDto.Name
        };
    }

    public static GenreDto ToDto( this Genre genre){
        return new GenreDto(
            genre.Id,
            genre.Name
        );
    }
}
