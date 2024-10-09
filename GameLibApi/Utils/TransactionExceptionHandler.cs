
using Npgsql;

public static class TransactionExceptionHandler{

    public static IResult Handle(Exception exception){

        if ( exception.InnerException  is PostgresException postgresEx){
            switch(postgresEx.SqlState){

                case PostgresErrorCodes.UniqueViolation: 
                    return Results.Conflict("An entry with the same name already exists.");

                default:
                    break;

            }
        }
        return Results.InternalServerError();

    }
}