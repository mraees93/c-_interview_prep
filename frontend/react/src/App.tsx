import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import ApiConsumption from './components/ApiConsumption';
import PostData from './components/PostData';

export default function App() {
  return (
    <BrowserRouter>
    <nav>
        <Link to="/">Home</Link> |{" "}
        <Link to="/form">Form</Link> |{" "}
        <Link to="/contact">Contact</Link>
      </nav>

      <Routes>
        <Route path="/" element={ <ApiConsumption /> } />
        <Route path="/form" element={<PostData />} />
        {/* <Route path="/contact" element={<Contact />} /> */}
      </Routes>
    </BrowserRouter>
  );
}
