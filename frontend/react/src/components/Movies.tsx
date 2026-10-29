import * as React from 'react';
import { useEffect, useState } from 'react';
const token = (import.meta as any).VITE_TMDB_TOKEN;

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

        const response: Response = await fetch('https://themoviedb.org', options);
        if(!response.ok) {
          throw new Error(`HTTP network error: ${response.status}`)
        }

        const data = await response.json();
        setData(data.results);
      } catch (error: unknown) {

        if (error instanceof Error) {
          setError(error.message)
        } else if (typeof error === "string") {
          setError(error || "Failed to populate from API")
        } 
        // else {
        //   setError()
        // }

      } finally {

      }
    }
  }, [])

  return (
    <div>
      
    </div>
  );
}
