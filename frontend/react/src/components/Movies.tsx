import * as React from 'react';
import { useEffect, useMemo, useState, type ChangeEvent } from 'react';
const token = (import.meta as any).env.VITE_TMDB_TOKEN;
const IMAGE_BASE_URL = "https://image.tmdb.org/t/p/w500";

import {TMDBGenre} from '../util';

export interface TMDBMovie {
  id: number;
  title: string;
  poster_path: string | null;
  vote_average: number;
  overview: string;
  release_date: string;
  genre_ids: number[]
}

interface TMDBResponse {
  page: number;
  results: TMDBMovie[];
  total_pages: number;
  total_results: number;
}

const SORT_METHODS: Record<string, (a: TMDBMovie, b: TMDBMovie) => number> = {

  original: () => 0,

  alphabetical: (a, b) => a.title.localeCompare(b.title),

  rating: (a, b) => b.vote_average - a.vote_average,

  releaseDate: (a, b) => new Date(b.release_date).getTime() - new Date(a.release_date).getTime(),
}

export default function Movies () {

  const [data, setData] = useState<TMDBMovie[]>([]);
  const [error, setError] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [currentSort, setCurrentSort] = useState<string>("original");
  const [genreSearchTerm, setGenreSearchTerm] = useState<string>('');

  useEffect(()=> {
    const fetchMovies = async () => {
      const options = {
        method: 'GET',
        headers: {
          accept: 'application/json',
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
        } else {
          setError("An unexpected system exception occured.");
        }

      } finally {
        setIsLoading(false);
      }
    }
    fetchMovies();
  }, [])
  
  const visibleMovies = currentSort === "original" ? data : [...data].sort(SORT_METHODS[currentSort]);

  // TODO: Add sorting functionality for crucial performance, use debounce pattern as well
  const processedMovies = useMemo<TMDBMovie[]>(() => {
      const userSearch = genreSearchTerm.toLowerCase().trim();

      let result = [...visibleMovies];

      if(!userSearch) return result;

      return result.filter((movie) => {
        if(!movie.genre_ids) return false;

        return movie.genre_ids.some((id) => {
          const genreName = TMDBGenre[id]?.toLowerCase() || '';
          return genreName.includes(userSearch);
        })
      })
      
  }, [visibleMovies, genreSearchTerm]);
  
  const handleGenreSearch = (event: ChangeEvent<HTMLInputElement>) => {
      setGenreSearchTerm(event.target.value);
  }

  return (
    <div>
      <div>
            <label htmlFor="search-by-genre">Search by genre:</label>
            <input
                id="search-by-genre"
                type="text"
                value={genreSearchTerm}
                onChange={handleGenreSearch}
                placeholder="Search by genre"
            />
      </div>
      <label htmlFor="sort-select" style={{ marginRight: '10px', fontWeight: 'bold' }}></label>
      <select
        id="sort-select"
        value={currentSort}
        onChange={(e) => setCurrentSort(e.target.value)}
        style={{ padding: '8px', borderRadius: '4px', marginBottom: '20px' }}
      >
        <option value="original">Default (Trending)</option>
        <option value="alphabetical">Title (A-Z)</option>
        <option value="rating">Rating (Highest First)</option>
        <option value="releaseDate">Release Date (Newest First)</option>
      </select>

      {isLoading && (
        <div><p><strong>Movies are loading...</strong></p></div>
      )}

      {error && (
        <div><p><strong>{error}</strong></p></div>
      )}

      {!isLoading && !error && (
        <ul>
          {
            data.length === 0 ? (
              <div><p>No movies found</p></div>
            ) 
            
            : 

            (
              visibleMovies.map((movie: TMDBMovie) => 
              <div key = {movie.id}>
                {movie.poster_path ? (
                  <img
                  src={`${IMAGE_BASE_URL}${movie.poster_path}`}
                  alt={`${movie.title} Poster`}
                  style={{ width: '25%', height: '10%', borderRadius: '8px' }}
                  />
                ) : (
                  <div><p>No image available</p></div>
                )}
                <div>{movie.title}</div>
              </div>
              )
            )
          }
        </ul>
      )}
    </div>
  );
}
