import React from "react";

const SearchForm = ({ handleCancel }) => {
  return (
    <div className="search-form">
      <h2>Formulario de Búsqueda</h2>
      <p>Este es el formulario de búsqueda</p>
      <button onClick={handleCancel} className="button cancel">
        Cancelar
      </button>
    </div>
  );
};

export default SearchForm;
