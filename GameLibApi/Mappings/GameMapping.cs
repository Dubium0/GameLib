using System;
using GameLibApi.Dtos.GameDtos;
using GameLibApi.Entities;

namespace GameLibApi.Mappings;

public static class GameMapping
{
    public static Game ToEntity(this CreateGameDto game){

        return new Game{
                Name = game.Name,
                ReleaseDate = game.ReleaseDate,
                MetaCritic = game.MetaCritic,
                BackgroundImageUrl = game.BackgroundImageUrl
            };

    }

    public static Game ToEntity(this UpdateGameDto game,int id){

         return new Game{
                Id = id,
                Name = game.Name,
                ReleaseDate = game.ReleaseDate,
                MetaCritic = game.MetaCritic,
                BackgroundImageUrl = game.BackgroundImageUrl
            };

    }
    public static GameSummaryDto ToSummaryDto(this Game game,List<string> genres, List<string> platforms){
        return new GameSummaryDto(
            game.Id, 
            game.Name, 
            genres,
            platforms,
            game.ReleaseDate,
            game.MetaCritic,
            game.BackgroundImageUrl
        );
    }
    public static GameDetailsDto ToDetailsDto(this Game game,List<int> genreIds, List<int> platformIds){
        return new GameDetailsDto(
            game.Id, 
            game.Name, 
            genreIds,
            platformIds,
            game.ReleaseDate,
            game.MetaCritic,
            game.BackgroundImageUrl
        );
    }

}
