import React, { useState } from "react";
import axios from "axios";
import "./SearchForm.css";


const SearchForm = ({ handleCancel }) => {
  const [results, setResults] = useState({
    monumentos: [],
    localidades: [],
    provincias: [],
  });

  const [filters, setFilters] = useState({
    localidad: "",
    codPostal: "",
    provincia: "",
    tipo: "all",
  });

  const handleInputChange = (e) => {
    const { id, value } = e.target;
    setFilters((prevFilters) => ({
      ...prevFilters,
      [id]: value,
    }));
  };

  const handleSearch = async () => {
    const endpoints = {
      getAllDatabase: "http://localhost:5005/api/busqueda/getAllDatabase",
    };

    try {
      const response = await axios.get(endpoints.getAllDatabase);
      const data = response.data;

      console.log(data);

      setResults({
        monumentos: data.monumentos || [],
        localidades: data.localidades || [],
        provincias: data.provincias || [],
      });
    } catch (error) {
      console.error("Error durante la búsqueda:", error);
      setResults({
        monumentos: [],
        localidades: [],
        provincias: [],
      });
    }
  };

  const getLocalidadNombre = (idLocalidad) => {
    const localidad = results.localidades.find(
      (loc) => loc.idLocalidad === idLocalidad
    );
    return localidad ? localidad.nombre : "Desconocido";
  };

  const getProvinciaNombre = (idLocalidad) => {
    // Buscar la localidad en la lista de localidades
    const localidad = results.localidades.find(
      (loc) => loc.idLocalidad === idLocalidad
    );
  
    if (!localidad) {
      return "Desconocido"; // Si no se encuentra la localidad
    }
  
    // Buscar la provincia en la lista de provincias usando el idProvincia de la localidad
    const provincia = results.provincias.find(
      (prov) => prov.idProvincia === localidad.idProvincia
    );
  
    return provincia ? provincia.nombre : "Desconocido"; // Retornar el nombre de la provincia si se encuentra
  };
  

  return (
    <div className="search-form-container">
      {/* Sección superior: Formulario y mapa */}
      <div className="top-section">
        <div className="form-section">
          <h2>Formulario de Búsqueda</h2>
          <div className="form-group">
            <label htmlFor="localidad">Localidad:</label>
            <input
              type="text"
              id="localidad"
              value={filters.localidad}
              onChange={handleInputChange}
              placeholder="Ej. Burgos"
            />
          </div>
          <div className="form-group">
            <label htmlFor="codPostal">Cod. Postal:</label>
            <input
              type="text"
              id="codPostal"
              value={filters.codPostal}
              onChange={handleInputChange}
              placeholder="Ej. 09003"
            />
          </div>
          <div className="form-group">
            <label htmlFor="provincia">Provincia:</label>
            <input
              type="text"
              id="provincia"
              value={filters.provincia}
              onChange={handleInputChange}
              placeholder="Ej. Castilla y León"
            />
          </div>
          <div className="form-group">
            <label htmlFor="tipo">Tipo:</label>
            <select
              id="tipo"
              value={filters.tipo}
              onChange={handleInputChange}
            >
              <option value="all">Todos</option>
              <option value="historical">Histórico</option>
              <option value="modern">Moderno</option>
              <option value="natural">Natural</option>
            </select>
          </div>
          <div className="form-actions">
            <button onClick={handleSearch} className="button search">
              Buscar
            </button>
            <button onClick={handleCancel} className="button cancel">
              Cancelar
            </button>
          </div>
        </div>
        <div className="map-section">
          <h2>Mapa</h2>
          <div className="map-placeholder">
            <p>Aquí irá el mapa interactivo</p>
          </div>
        </div>
      </div>

      {/* Sección inferior: Resultados */}
      <div className="results-section">
        <h2>Resultados</h2>
        {results.monumentos.length > 0 ? (
          <table className="results-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Dirección</th>
                <th>Localidad</th>
                <th>Cod. Postal</th>
                <th>Provincia</th>
                <th>Descripción</th>
              </tr>
            </thead>
            <tbody>
              {results.monumentos.map((monumento, index) => (
                <tr key={index}>
                  <td>{monumento.nombre}</td>
                  <td>{monumento.tipo}</td>
                  <td>{monumento.direccion}</td>
                  <td>{getLocalidadNombre(monumento.idLocalidad)}</td>
                  <td>{monumento.codigoPostal}</td>
                  <td>{getProvinciaNombre(monumento.idLocalidad)}</td>
                  <td>{monumento.descripcion}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className="no-results">No se encontraron resultados</p>
        )}
      </div>
    </div>
  );
};

export default SearchForm;
