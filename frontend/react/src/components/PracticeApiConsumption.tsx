
import { useEffect, useMemo, useState } from 'react';
import './ApiConsumption.css';

interface CommentLog {
    id: number;
    name: string;
    email: string;
    body: string;
}

export default function Practice() {

    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [data, setData] = useState<CommentLog[]>([]);
    const [sortDirection, setSortDirection] = useState<"ASC" | "DESC">("ASC");
    const [searchTerm, setSearchTerm] = useState<string>('');


    useEffect(() => {
        const fetchData = async () => {

            try {
                setError(null);
                setIsLoading(true);

                const response = await fetch("https://jsonplaceholder.typicode.com/comments");
                if(!response.ok) {
                    throw new Error("Something went wrong fetching the data")
                }

                const data = await response.json();
                setData(data)
            } catch (error: unknown) {

                if(error instanceof Error) {
                    setError(error.message);
                } else if (typeof error === "string") {
                    setError(error);
                } else {
                    setError("Something went wrong fetching the data")
                }

            } finally {
                setIsLoading(false);
            }
        }
        fetchData();
    }, [])

    const toggleSort = () => setSortDirection(prev => prev === "ASC" ? "DESC" : "ASC"); 

    // const processedComments = useMemo<CommentLog[]>(() => {
    //     const search = searchTerm.toLowerCase().trim();
    //     const isAsc = sortDirection === "ASC";
    // })

    return (
        <div>
            {isLoading && <div>Fetching data...</div>}
            {error && <div>{error}</div>}
            {data.length === 0 
                ? <p>No comments available</p>
                : <div>
                    {
                        data.map((comment) => <div key={comment.id}>{comment.name}</div>)
                    }
                </div>
            }
        </div>
    )
}