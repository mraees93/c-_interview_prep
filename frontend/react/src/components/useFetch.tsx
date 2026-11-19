import { useEffect, useState } from "react"


// custom re-usable hook

export default function useFetch(url: string) {
    const [data, setData] = useState<[]>([]);

    useEffect(() => {
        fetch(url)
            .then((res) => res.json())
            .then((data) => setData(data));
    }, [url]);

    return [data];
};

/*
TODO:
usage in different component:

const [data] = useFetch(...url...);

*/