
import { useEffect, useMemo, useState, type ChangeEvent as changeEvent } from 'react';
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

    const processedComments = useMemo<CommentLog[]>(() => {
        const search = searchTerm.toLowerCase().trim();
        const isAsc = sortDirection === "ASC";

        let result = [...data];
        if(search) {
            result = result.filter(item => {
                const nameMatch = item.name ? item.name.toLowerCase().includes(search) : false;
                const emailMatch = item.email ? item.email.toLowerCase().includes(search) : false;
                return nameMatch || emailMatch;
            })
        }

        return result.sort((a, b) => isAsc ? a.id - b.id : b.id - a.id);
    }, [data, sortDirection, searchTerm]);

    const handleSearch = (event: changeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
    }

    return (
        <div>
            <div>
                <label htmlFor='search'>Filter comments:</label>
                <input
                    id='search'
                    type='text'
                    value={searchTerm}
                    onChange={handleSearch}
                    placeholder='Search by name or email...'
                />
                <button onClick={toggleSort}>
                    Sort by: {sortDirection === "ASC" ? "oldest" : "newest"}
                </button>
            </div>
            {isLoading && <div>Fetching data...</div>}
            {error && <div>{error}</div>}
            {!isLoading && !error && processedComments.length === 0 
                ? <p>No comments found</p>
                : <div>
                    {
                        processedComments.map((comment) => 
                        <div key={comment.id}>
                            {comment.id}
                            {" "}
                            {comment.name}
                            {" "}
                            {comment.email}
                        </div>
                        )
                    }
                </div>
            }
        </div>
    )
}