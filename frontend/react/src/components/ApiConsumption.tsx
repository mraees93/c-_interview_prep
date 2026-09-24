import React, { useState, useEffect, useMemo, type ChangeEvent as changeEvent } from 'react';
import './ApiConsumption.css';

export interface CommentLog {
    id: number;
    name: string;
    email: string;
    body: string;
}

export default function ApiConsumption() {
    // CONCEPT: Typed State Hooks (useState<T> maps directly to signal<T>)
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [sortDirection, setSortDirection] = useState<'ASC' | 'DESC'>('ASC');

    // async state machine hooks
    const [comments, setComments] = useState<CommentLog[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    // CONCEPT: Lifecycle Synchronization (useEffect maps to constructor effect)
    useEffect(() => {
        const fetchComments = async () => {
            try{
                setIsLoading(true);

                const response = await fetch("https://jsonplaceholder.typicode.com/comments");

                if(!response.ok) {
                    throw new Error(`HTTP network error! Status: ${response.status}`);
                }

                const data: CommentLog[] = await response.json();
                setComments(data);

            } catch (err: unknown) {

                if(err instanceof Error) {
                    setError(err.message);
                } else {
                    setError('Failed to populate data metrics from server.');
                }

            } finally {
                setIsLoading(false);
            }
        };

        fetchComments();
    }, [])

    // CONCEPT: Memory Matrix Caching (useMemo maps to computed)
    const processedComments = useMemo<CommentLog[]>(() => {
        const search = searchTerm.toLowerCase().trim();
        const isAsc = sortDirection === 'ASC';

        let result = comments;
        if(search) {
            result = comments.filter(item => {
                const nameMatch = item.name ? item.name.toLowerCase().includes(search) : false;
                const emailMatch = item.email ? item.email.toLowerCase().includes(search) : false;
                return nameMatch || emailMatch;
            })
        }

        return [...result].sort((a, b) => {
            return isAsc ? a.id - b.id : b.id - a.id;
        })
    }, [comments, searchTerm, sortDirection]);

    const handleSearch = (event: changeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
    }

    const toggleSort = () => {
        setSortDirection(prev => prev === 'ASC' ? 'DESC' : 'ASC');
    }

    return (
    <div className="api-container">
      
      {/* Controller Toolbelt Grid */}
      <div className="control-panel">
        <div className="search-box">
          <label htmlFor="search">Live Filter Logs (React TS-driven):</label>
          <input
            id="search"
            type="text"
            value={searchTerm}
            onChange={handleSearch}
            placeholder="Search by name or email..."
          />
        </div>

        <button className="sort-btn" onClick={toggleSort}>
          Sort ID: {sortDirection === 'ASC' ? '▲ Oldest' : '▼ Newest'}
        </button>
      </div>

      {/* 1. Handle Loading State */}
      {isLoading && (
        <div className="loading-state">
          <p>Streaming data matrices down via React useEffect()...</p>
        </div>
      )}

      {/* 2. Handle Error State */}
      {error && (
        <div className="error-state">
          <p><strong>System Exception:</strong> {error}</p>
        </div>
      )}

      {/* 3. Handle Successful Data Render */}
      {!isLoading && !error && (
        <>
          <div className="meta-row">
            <p>Showing <strong>{processedComments.length}</strong> of 500 records found</p>
          </div>

          <div className="cards-grid">
            {processedComments.length > 0 ? (
              processedComments.map((item: CommentLog) => (
                <div className="comment-card" key={item.id}>
                  <div className="card-header">
                    <span className="id-badge">#{item.id}</span>
                    <span className="email-text">{item.email}</span>
                  </div>
                  <h4>{item.name}</h4>
                  <p>{item.body}</p>
                </div>
              ))
            ) : (
              <div className="empty-state">
                <p>No comments match your search criteria. Try a different query term.</p>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}