import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import ApiConsumption from './components/ApiConsumption';
import PostData from './components/PostData';
import Movies from './components/Movies';

export default function App() {
  return (
    <BrowserRouter>
    <nav>
        <Link to="/">Home</Link> |{" "}
        <Link to="/form">Form</Link> |{" "}
        <Link to="/movies">Movies</Link>
      </nav>

      <Routes>
        <Route path="/" element={ <ApiConsumption /> } />
        <Route path="/form" element={<PostData />} />
        <Route path="/movies" element={<Movies />} /> 
      </Routes>
    </BrowserRouter>
  );
}
