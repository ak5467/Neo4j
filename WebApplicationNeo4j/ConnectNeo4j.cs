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

        /// <summary>
        /// returns movies based on user profile selected
        /// </summary>
        /// <param name="userSK"></param>
        /// <returns></returns>
        public List<MovieDim> MoviesBasedOnUserProfile(int userSK)
        {
            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(p1: UserDim { UserSK: {userSK}})< -[x: ratedUser] - (f1: FactTable) -[r1: ratedMovie]->(m: MovieDim) < -[r2: ratedMovie] - (f2: FactTable) -[y: ratedUser]->(p2: UserDim)")
                .With("count(m) as movieCount, collect(m.MovieSK) as mov, SUM(f1.Rating * f2.Rating) AS xyDotProduct, " +
                "SQRT(REDUCE(xDot = 0.0, a IN COLLECT(f1.Rating) | xDot + a ^ 2)) AS xLength, " +
                "SQRT(REDUCE(yDot = 0.0, b IN COLLECT(f2.Rating) | yDot + b ^ 2)) AS yLength, " +
                "p1, p2")
                .Where("movieCount> 10")
                .With("p1, p2, xyDotProduct / (xLength * yLength) as similarity, mov")
                .OrderByDescending("similarity")
                .Limit(5)
                .OptionalMatch("(p2)< -[a: ratedUser] - (f3: FactTable) -[b: ratedMovie]->(m1: MovieDim)")
                .With("f3.MovieSK as fact_movieSK, mov, m1.MovieSK as movieSK, m1 as Movies")
                .Where("NOT movieSK IN mov")
                .WithParam("userSK", userSK)
                .Return<MovieDim>("Movies")
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

        /// <summary>
        /// returns movies with most common Genres
        /// 
        /// </summary>
        /// <param name="movieSK"></param>
        /// <returns></returns>
        public List<MovieDim> SameGenreMovies(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim{MovieSK:{SK}}) -[:belongsToGenre]->(g:GenreDim) <-[:belongsToGenre]-(simMov:MovieDim)")
                .With("simMov, collect(g.GenreName) as genre, count(*) as number")
                .WithParam("SK", movieSK)
                .OrderByDescending("number")
                .Return<MovieDim>("simMov")
                .Limit(5)
                .Results.ToList();

            return Movies;
        }

        public List<MovieDim> SimilarWeightedMovie(int movieSK)
        {

            List<MovieDim> Movies = GraphConfig.GraphClient.Cypher
                .Match("(m:MovieDim {MovieSK: {SK}} )")
                .Match("(m) -[:belongsToGenre]->(g: GenreDim) < -[:belongsToGenre] - (simMov: MovieDim)")
                .With("m, simMov, COUNT(*) AS gCount")
                .OptionalMatch("(m)< -[:hasATag] - (a: TagDim) -[:hasATag]->(simMov)")
                .With("m, simMov, gCount, COUNT(a) AS tCount")
                .OptionalMatch("(m)< -[:ratedMovie] - (d: FactTable) -[:ratedMovie]->(simMov)")
                .With("m, simMov, gCount, tCount, COUNT(d) AS rCount")
                .With("simMov AS movies, (5* gCount)+(2* tCount)+(3* rCount) AS Weight")
                .WithParam("SK", movieSK)
                .Return<MovieDim>("movies")
                .OrderByDescending("Weight")
                .Limit(5)
                .Results.ToList();
 
            return Movies;
        }
    }
}