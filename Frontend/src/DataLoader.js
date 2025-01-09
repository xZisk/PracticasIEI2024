import React, { useState } from "react";
import axios from "axios";
import "./DataLoader.css";

const DataLoader = ({ handleCancel }) => {
  const [selectedSources, setSelectedSources] = useState([]);
  const [results, setResults] = useState({
    successfulRecords: 0,
    repairedRecords: [],
    rejectedRecords: [],
    deleteResults: null,
    action: null
  });

  const sources = [
    { id: "all", name: "Seleccionar todas" },
    { id: "castilla", name: "Castilla y León" },
    { id: "valenciana", name: "Comunitat Valenciana" },
    { id: "euskadi", name: "Euskadi" },
  ];

  const handleSourceChange = (id) => {
    if (id === "all") {
      if (selectedSources.length === sources.length) {
        setSelectedSources([]);
      } else {
        setSelectedSources(sources.map((s) => s.id));
      }
    } else {
      setSelectedSources((prev) => {
        let newSelectedSources = prev.includes(id) ? prev.filter((source) => source !== id) : [...prev, id];
        if (newSelectedSources.length === sources.length - 1 && !newSelectedSources.includes("all")) {
          newSelectedSources.push("all");
        } else if (newSelectedSources.includes("all") && newSelectedSources.length < sources.length) {
          newSelectedSources = newSelectedSources.filter((source) => source !== "all");
        }
        return newSelectedSources;
      });
    }
  };

  const handleLoad = async () => {
    const endpoints = {
      all: "http://localhost:5000/api/carga/all",
      castilla: "http://localhost:5000/api/carga/xml",
      valenciana: "http://localhost:5000/api/carga/csv",
      euskadi: "http://localhost:5000/api/carga/json",
    };

    try {
      let newResults = {
        successfulRecords: 0,
        repairedRecords: [],
        rejectedRecords: [],
        deleteResults: null,
        action: "load"
      };

      const sourcesToLoad = selectedSources.includes("all") ? ["all"] : selectedSources;

      for (const source of sourcesToLoad) {
        const response = await axios.post(endpoints[source]);
        const data = response.data;
        console.log("Response received:", response); // Agregar detalles de depuración
        newResults.successfulRecords += data.successfulRecords || 0;
        newResults.repairedRecords = [...newResults.repairedRecords, ...(data.repairedRecords || [])];
        newResults.rejectedRecords = [...newResults.rejectedRecords, ...(data.rejectedRecords || [])];
      }
      console.log("newResults:", newResults);
      setResults(newResults);
    } catch (error) {
      console.error("Error loading data:", error);
    }
  };

  const handleClear = async () => {
    try {
      console.log("Attempting to clear data...");
      const response = await axios.delete("http://localhost:5000/api/carga/clear", { data: {} });
      console.log("Response received:", response);
      if (response.status === 200) {
        const data = response.data;
        console.log("Data cleared successfully.");
        setSelectedSources([]);
        setResults({
          successfulRecords: 0,
          repairedRecords: [],
          rejectedRecords: [],
          deleteResults: Object.fromEntries(Object.entries(data).filter(([key]) => key !== "sqlite_sequence")),
          action: "clear"
        });
      } else {
        console.error("Error clearing data:", response.statusText);
      }
    } catch (error) {
      console.error("Error clearing data:", error);
    }
  };

  return (
    <div className="data-loader">
      <p>Carga del almacén de datos</p>
      <div className="source-selection">
        <p>Seleccione fuente:</p>
        <div className="source-list">
          {sources.map((source) => (
            <label key={source.id} className="source-item">
              <input
                type="checkbox"
                checked={
                  source.id === "all"
                    ? selectedSources.length === sources.length
                    : selectedSources.includes(source.id)
                }
                onChange={() => handleSourceChange(source.id)}
              />
              {source.name}
            </label>
          ))}
        </div>
      </div>

      <div className="actions">
        <button onClick={handleCancel} className="button cancel">
          Cancelar
        </button>
        <button onClick={handleLoad} className="button load">
          Cargar
        </button>
        <button onClick={handleClear} className="button clear">
          Borrar almacén de datos
        </button>
        <p>Resultados de la carga:</p>
      </div>

      <div className="results">
        <div className="results-container">
          {results.action === "load" && (
            <>
              <p>
                <strong>Número de registros cargados correctamente:</strong> {results.successfulRecords}
              </p>
              <p>
                <strong>Registros con errores y reparados:</strong>
              </p>
              <ul>
                {results.repairedRecords.map((record, index) => (
                  <li key={index}>
                    {record}
                  </li>
                ))}
              </ul>
              <p>
                <strong>Registros con errores y rechazados:</strong>
              </p>
              <ul>
                {results.rejectedRecords.map((record, index) => (
                  <li key={index}>
                    {record}
                  </li>
                ))}
              </ul>
            </>
          )}
          {results.action === "clear" && results.deleteResults && (
            <div>
              <p><strong>Resultados de la eliminación:</strong></p>
              <ul>
                {Object.entries(results.deleteResults).map(([table, count], index) => (
                  <li key={index}>
                    {table}: {count} registros eliminados
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default DataLoader;
