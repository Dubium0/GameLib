using System;
using GameLibApi.Dtos.PlatformDtos;
using GameLibApi.Entities;


namespace GameLibApi.Mappings;
public static class PlatformMapping
{
    public static Platform ToEntity(this UpdatePlatformDto platformDto, int id){
            return new Platform(){
            Id = id,
            Name = platformDto.Name
        };
    }

    public static Platform ToEntity(this CreatePlatformDto platformDto){
        return new Platform() {
            Name = platformDto.Name
        };
    }

    public static PlatformDto ToDto( this Platform platform){
        return new PlatformDto(
            platform.Id,
            platform.Name
        );
    }
}
