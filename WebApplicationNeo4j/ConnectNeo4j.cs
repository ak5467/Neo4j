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

        public MovieDim GetMovie(int SK)
        {
            List <MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim)")
                .Where("(m.MovieSK = {sk})")
                .WithParam("sk", SK)
                .Return<MovieDim>("m")
                .Results.ToList();

            return Movies[0];
        }

        public String[] GetGenre(int movieSK)
        {
            //find rating of the movie
            List<String> Genre = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK:{MovieSK} })")
                .OptionalMatch("(m)-[r]->(g:GenreDim)")
                .WithParam("MovieSK", movieSK)
                .Return<String>("g.GenreName")
                .Results.ToList();
            String[] arr = Genre.ToArray();
            return arr;
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

        public List<MovieDim> SimilarMovies(double Rating, String GenreName, int SK)
        {            
            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(f:FactTable)")
                .With("round(100*avg(f.Rating))/100 as avgRate, f.MovieSK as movieSK")
                .Match("(f:FactTable)-[:ratedMovie]->(m:MovieDim{MovieSK:movieSK})")
                .Where("avgRate = {rate}")
                .With("m as simMov")
                .Match("(simMov)-[:belongsToGenre]->(g:GenreDim{GenreName:{name}})")
                .With("simMov where simMov IS NOT NULL and simMov.MovieSK <> {sk}")
                .WithParams(new { rate = Rating, name = GenreName, sk = SK })
                .Return<MovieDim>("distinct simMov")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SimilarUserMovies(int movieSK, double rating)
        {
            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim{MovieSK:{SK}})<-[:ratedMovie]-(f:FactTable)-[:ratedUser]->(u:UserDim)")
                .Where("f.Rating>= {rate} and f.UserSK = u.UserSK")
                .With("u as users, collect(f.Rating) as ratings")
                .OrderByDescending("ratings")
                .Match("(users)<-[:ratedUser]-(f1:FactTable)-[:ratedMovie]->(m1:MovieDim)")
                .Where("f1.Rating in ratings")
                .WithParams(new { rate = rating, SK = movieSK })
                .Return<MovieDim>("distinct m1")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SameGenreMovies(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim{MovieSK:{SK}}) -[:belongsToGenre]->(g:GenreDim) <-[:belongsToGenre]-(simMov:MovieDim)")
                .With("simMov, collect(g.GenreName) as genre, count(*) as number")
                .WithParam("SK", movieSK)
                .OrderByDescending("number")
                .Return<MovieDim>("distinct simMov")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SameTagMovies(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK: {SK}} )-[r:hasATag]->(t:TagDim)")
                .With("t as tags")
                .OrderByDescending("r.relevance")
                .Limit(5)
                .Match("(tags)<-[r2:hasATag]-(simMov:MovieDim)")
                .Where("simMov.MovieSK <> {SK} and simMov is Not Null")
                .WithParam("SK", movieSK)
                .With("distinct simMov as Movies, r2.relevance as rel")
                .Return<MovieDim>("Movies")
                .OrderByDescending("rel")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }
    }
}