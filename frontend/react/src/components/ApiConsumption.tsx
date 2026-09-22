// import React, { useState, useEffect, useMemo, ChangeEvent } from 'react';
// import './ApiConsumption.css';

// export interface CommentLog {
//     id: number;
//     name: string;
//     email: string;
//     body: string;
// }

// export default ApiConsumption() {
//     // CONCEPT: Typed State Hooks (useState<T> maps directly to signal<T>)
//     const [searchTerm, setSearchTerm] = useState<string>('');
//     const [sortDirection, setSortDirection] = useState<'ASC' | 'DESC'>('ASC');

//     // async state machine hooks
//     const [comments, setComments] = useState<CommentLog[]>([]);
//     const [isLoading, setIsLoading] = useState<boolean>(false);
//     const [error, setError] = useState<string | null>(null);

//     // CONCEPT: Lifecycle Synchronization (useEffect maps to constructor effect)
//     useEffect(() => {
//         const fetchComments = async () => {
//             try{
//                 setIsLoading(true);

//                 const response = await fetch("https://jsonplaceholder.typicode.com/comments");

//                 if(!response.ok) {
//                     throw new Error(`HTTP network error! Status: ${response.status}`);
//                 }

//                 const data: CommentLog[] = await response.json();
//                 setComments(data);

//             } catch (err: unknown) {

//                 if(err instanceof Error) {
//                     setError(err.message);
//                 } else {
//                     setError('Failed to populate data metrics from server.');
//                 }

//             } finally {
//                 setIsLoading(false);
//             }
//         };

//         fetchComments();
//     }, [])

//     // CONCEPT: Memory Matrix Caching (useMemo maps to computed)
//     const processedComments = useMemo<CommentLog[]>(() => {
//         const search = searchTerm.toLowerCase().trim();
//         const isAsc = sortDirection === 'ASC';

//         let result = comments;
//         if(search) {
//             result = comments.filter(item => {
//                 const nameMatch = item.name ? item.name.toLowerCase().includes(search) : false;
//                 const emailMatch = item.email ? item.email.toLowerCase().includes(search) : false;
//                 return nameMatch || emailMatch;
//             })
//         }

//         return [...result].sort((a, b) => {
//             return isAsc ? a.id - b.id : b.id - a.id;
//         })
//     }, [comments, searchTerm, sortDirection]);
// }