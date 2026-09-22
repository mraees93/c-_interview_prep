import ApiConsumption from './components/ApiConsumption';

export default function App() {
  return (
    <div className="sandbox-container">
      <h1>React Concepts Sandbox</h1>
      <hr />
      {/* This renders our active practice component, matching the Angular layout */}
      <ApiConsumption />
    </div>
  );
}
