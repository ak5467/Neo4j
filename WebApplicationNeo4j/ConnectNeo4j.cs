using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationNeo4j
{
    public class ConnectNeo4j
    {
        public static IGraphClient GraphClient;

        public void Connection()
        {
            GraphClient = GraphConfig.ConfigGraph();
        }

        public List<MovieDim> SearchMovies(String keyword)
        {
            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim)")
                .Where("(m.Title =~ {Title})")
                .WithParam("Title", "(?i).*" + keyword + ".*")
                .Return<MovieDim>("m")
                .Results.ToList();

            return Movies;

        }

        public double Rating(int movieSK)
        {
            //find rating of the movie
            var Rating = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK:{MovieSK} })")
                .OptionalMatch("(m)<-[r]-(f:FactTable)")
                .WithParam("MovieSK", movieSK)
                .Return((f) => new
                {
                    rating = Return.As<String>("round(100*avg(f.Rating))/100")
                })
                .Results.ToList();

            return Convert.ToDouble(Rating[0].rating); 
        }

        public List<MovieDim> SimilarMovies(double Rating)
        {
            Array MovieArray = GraphConfig.GraphClient.Cypher
                .Match("(f:FactTable)")
                .With("round(100*avg(f.Rating))/100 as avgRate, f.MovieSK as movieSK")
                .Where("avgRate = {rating}")
                .WithParam("rating", Rating)
                .Return<int>("movieSK")
                .Limit(5)
                .Results.ToArray();

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim)")
                .Where("m.MovieSK in {MovieArray}")
                .WithParam("MovieArray", MovieArray)
                .Return<MovieDim>("m")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SimilarUserMovies(int movieSK, double rating)
        {
            var UserSK = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK:{MovieSK}})")
                .OptionalMatch("(m)<-[r]-(f:FactTable)")
                .Where("f.Rating = {rat}")
                .WithParams(new { MovieSK = movieSK, rat = rating })
                .Return<int>("f.UserSK")
                .Limit(1)
                .Results.ToList();

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(f:FactTable {UserSK:{SK}})")
                .OptionalMatch("(f)-[r]->(m:MovieDim)")
                .WithParam("SK", Convert.ToInt32(UserSK[0]))
                .Return<MovieDim>("m")
                .Limit(6)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SameGenreMovies(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK: {SK}} )")
                .OptionalMatch("(m)-[r1]->(g:GenreDim)<-[r2]-(s:MovieDim)")
                .WithParam("SK", movieSK)
                .Return<MovieDim>("s")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SameTagMovies(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK: {SK}} )")
                .OptionalMatch("(m)-[r1]->(g:TagDim)<-[r2]-(s:MovieDim)")
                .WithParam("SK", movieSK)
                .Return<MovieDim>("s")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }
    }
}