import React, { useState, useEffect } from "react";
import axios from "axios";
import "./SearchForm.css";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "leaflet-draw/dist/leaflet.draw.css";
import "leaflet-draw";

const SearchForm = ({ handleCancel }) => {
  const [map, setMap] = useState(null);
  const [drawnItems, setDrawnItems] = useState(new L.FeatureGroup());
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

  // Inicialización del mapa con Leaflet
  useEffect(() => {
    const initializedMap = L.map("map").setView([40.416775, -3.703790], 6); // Madrid

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: '© OpenStreetMap contributors',
    }).addTo(initializedMap);

    const items = new L.FeatureGroup();
    initializedMap.addLayer(items);
    setDrawnItems(items);

    const drawControl = new L.Control.Draw({
      edit: {
        featureGroup: items,
      },
      draw: {
        polyline: true,
        polygon: true,
        rectangle: true,
        circle: true,
        marker: true,
      },
    });

    initializedMap.addControl(drawControl);

    initializedMap.on(L.Draw.Event.CREATED, (event) => {
      const layer = event.layer;
      items.addLayer(layer);

      console.log("Dibujo creado:", layer.toGeoJSON());
    });

    initializedMap.addControl(drawControl);
    const customIcon = L.icon({
      iconUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png", // Puedes cambiar esta URL a una imagen personalizada
      iconSize: [22, 32], 
      iconAnchor: [16, 22], 
      popupAnchor: [0, -22], 
    });

    const puntos = [
      { lat: 40.416775, lng: -3.703790, title: "Punto 1" }, 
      { lat: 41.38879, lng: 2.15899, title: "Punto 2" },  
      { lat: 42.1401, lng: -0.4082, title: "Punto 3" }  
    ];

    puntos.forEach((punto) => {
      const marker = L.marker([punto.lat, punto.lng], { icon: customIcon }).addTo(initializedMap);
      marker.bindPopup(`<b>${punto.title}</b><br/>Lat: ${punto.lat}<br/>Lng: ${punto.lng}`);
    });

    setMap(initializedMap);

    const placeholder = document.querySelector(".map-placeholder > p");
    if (placeholder) placeholder.remove();

    return () => {
      initializedMap.remove();
    };
  }, []);

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
    const localidad = results.localidades.find(
      (loc) => loc.idLocalidad === idLocalidad
    );

    if (!localidad) {
      return "Desconocido";
    }

    const provincia = results.provincias.find(
      (prov) => prov.idProvincia === localidad.idProvincia
    );

    return provincia ? provincia.nombre : "Desconocido";
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
            <div id="map" style={{ height: "400px", width: "100%" }}></div>
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
