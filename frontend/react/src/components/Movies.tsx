import * as React from 'react';
import { useEffect, useState } from 'react';
const token = (import.meta as any).env.VITE_TMDB_TOKEN;

interface TMDBMovie {
  id: number;
  title: string;
  poster_path: string | null;
  vote_average: number;
  overview: string;
}

interface TMDBResponse {
  page: number;
  results: TMDBMovie[];
  total_pages: number;
  total_results: number;
}

export default function Movies () {

  const [data, setData] = useState<TMDBMovie[]>([]);
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);

  useEffect(()=> {
    const fetchMovies = async () => {
      const options = {
        method: 'GET',
        headers: {
          accept: 'application/json',
          // 2. Use that variable inside your template literal
          Authorization: `Bearer ${token}`
        }
      } ;

      try {
        setIsLoading(true);

        const response: Response = await fetch('https://api.themoviedb.org/3/movie/popular', options);
        if(!response.ok) {
          throw new Error(`HTTP network error: ${response.status}`)
        }

        const data = await response.json();
        setData(data.results);

      } catch (error: unknown) {

        if (error instanceof Error) {
          setError(error.message)
        } else if (typeof error === "string") {
          setError(error || "Failed to populate from API");
        } 
        else {
          setError("An unexpected system exception occured.");
        }

      } finally {
        setIsLoading(false);
      }
    }
    fetchMovies();
  }, [])

  useEffect(() => {
    console.log(isLoading, error, data);
    
  })

  return (
    <div>
      {isLoading && (
        <div><p><strong>Movies are loading...</strong></p></div>
      )}

      {error && (
        <div><p><strong>{error}</strong></p></div>
      )}
    </div>
  );
}
