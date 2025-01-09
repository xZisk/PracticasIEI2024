import React, { useState } from "react";
import DataLoader from "./DataLoader"; 
import SearchForm from "./SearchForm"; 
import './App.css';

function App() {
  const [showSearchForm, setShowSearchForm] = useState(false);
  const [showUploadForm, setShowUploadForm] = useState(false);

  const handleSearchClick = () => {
    setShowSearchForm(true);
    setShowUploadForm(false);
  };

  const handleUploadClick = () => {
    setShowSearchForm(false);
    setShowUploadForm(true);
  };

  const handleCancel = () => {
    setShowSearchForm(false);
    setShowUploadForm(false);
  };

  return (
    <div className="App">
      {!showSearchForm && !showUploadForm ? (
        <div className="welcome-screen">
          <h1>Bienvenido</h1>
          <p>Selecciona una opción para continuar:</p>
          <div className="buttons-container">
            <button className="button" onClick={handleSearchClick}>
              Formulario de Búsqueda
            </button>
            <button className="button" onClick={handleUploadClick}>
              Formulario de Carga
            </button>
          </div>
        </div>
      ) : (
        <div>
          {showSearchForm && <SearchForm handleCancel={handleCancel} />}
          {showUploadForm && <DataLoader handleCancel={handleCancel} />}
        </div>
      )}
    </div>
  );
}

export default App;
