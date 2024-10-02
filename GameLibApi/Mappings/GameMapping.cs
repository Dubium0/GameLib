using System;
using GameLibApi.Dtos.GameDtos;
using GameLibApi.Entities;

namespace GameLibApi.Mappings;

public static class GameMapping
{
    public static Game ToEntity(this CreateGameDto game){

         return new Game{
                Name = game.Name,
                GenreId = game.GenreId,
                PlatformId = game.PlatformId,
                ReleaseDate = game.ReleaseDate,
                MetaCritic = game.MetaCritic,
                BackgroundImageUrl = game.BackgroundImageUrl
            };

    }
    public static Game ToEntity(this UpdateGameDto game,int id){

         return new Game{
                Id = id,
                Name = game.Name,
                GenreId = game.GenreId,
                PlatformId = game.PlatformId,
                ReleaseDate = game.ReleaseDate,
                MetaCritic = game.MetaCritic,
                BackgroundImageUrl = game.BackgroundImageUrl
            };

    }
    public static GameSummaryDto ToSummaryDto(this Game game){
        return new GameSummaryDto(
            game.Id, 
            game.Name, 
            game.Genre!.Name, 
            game.Platform!.Name,
            game.ReleaseDate,
            game.MetaCritic,
            game.BackgroundImageUrl
        );
    }
    public static GameDetailsDto ToDetailsDto(this Game game){
        return new GameDetailsDto(
            game.Id, 
            game.Name, 
            game.GenreId, 
            game.PlatformId,
            game.ReleaseDate,
            game.MetaCritic,
            game.BackgroundImageUrl
        );
    }

}
