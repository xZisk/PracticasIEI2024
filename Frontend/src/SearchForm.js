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

  const [resultsTotales, setResultsTotales] = useState({
    monumentosCompletos: [],
  });

  const [filters, setFilters] = useState({
    localidad: "",
    codPostal: "",
    provincia: "",
    tipo: "all",
  });

  const [searchTrigger, setSearchTrigger] = useState(false);
  const [searchTriggerBusc, setSearchTriggerBusc] = useState(false);


  // Inicialización del mapa con Leaflet
  useEffect(() => {
    onOpen();
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
      iconUrl: "/marker-icon-blue.png",
      iconSize: [22, 32], 
      iconAnchor: [16, 22], 
      popupAnchor: [0, -22], 
    });

    const customIcon2 = L.icon({
      iconUrl: '/marker-icon-orange.png',
      iconSize: [22, 32], 
      iconAnchor: [16, 22], 
      popupAnchor: [0, -22], 
    });

    const puntos = [];

    resultsTotales.monumentosCompletos.forEach((monumento) => {
      const lat = parseFloat(monumento.latitud);
      const lng = parseFloat(monumento.longitud);
      
      if (!isNaN(lat) && !isNaN(lng)) {
        puntos.push({
          lat: lat,
          lng: lng,
          title: monumento.nombre,
          tipo: 0
        }); 
      } else {
        console.error("Latitud o longitud inválida:", monumento);
      }
    });
    

    if(searchTriggerBusc){
      console.log("ENTRA en search");
      results.monumentos.forEach((monumentoBuscado) => {
        console.log(monumentoBuscado.nombre);
        var puntoExistente = puntos.find((punto) => punto.title === monumentoBuscado.nombre);
        puntoExistente.tipo = 1;
      })
    }
  
    puntos.forEach((punto) => {
      var marker;
      if (punto.tipo === 0){
        marker = L.marker([punto.lat, punto.lng], {icon : customIcon}).addTo(initializedMap);
      } else {
        marker = L.marker([punto.lat, punto.lng], {icon : customIcon2}).addTo(initializedMap);
      }

      marker.bindPopup(`<b>${punto.title}</b><br/>Lat: ${punto.lat}<br/>Lng: ${punto.lng}`);
    });
    
    

    setMap(initializedMap);

    const placeholder = document.querySelector(".map-placeholder > p");
    if (placeholder) placeholder.remove();

    return () => {
      initializedMap.remove();
    };
  }, [searchTrigger, searchTriggerBusc]);

  const handleInputChange = (e) => {
    const { id, value } = e.target;
    setFilters((prevFilters) => ({
      ...prevFilters,
      [id]: value,
    }));
  };

  const onOpen = async () => {
    try {
      const endpoints = {
        getAllDatabase: "http://localhost:5005/api/busqueda/getAllDatabase",
      };
      var response = await axios.get(endpoints.getAllDatabase);
      var data = response.data;


      setResultsTotales({
        monumentosCompletos: data.monumentos || [],
      });

      setSearchTrigger(true);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  }

  const handleSearch = async () => {
    const endpoints = {
      getAllDatabase: "http://localhost:5005/api/busqueda/getAllDatabase",
      buscarMonumentos: "http://localhost:5005/api/busqueda/buscarMonumentos",
    };

    try {
      var response = await axios.get(endpoints.getAllDatabase);
      var data = response.data;

      setSearchTriggerBusc(false);
      setResults({
        monumentos: data.monumentos || [],
        localidades: data.localidades || [],
        provincias: data.provincias || [],
      });
    
      // Verifica si algún filtro tiene un valor
      if (shouldTriggerSearch(filters)) {
        try {
          response = await axios.get(endpoints.buscarMonumentos, { params: filters });
          data = response.data;  

          setSearchTriggerBusc(true);
          setResults((prevResults) => ({
            ...prevResults,
            monumentos: data.monumentos || [],
          }));
        } catch (error) {
          console.error("Error buscando monumentos:", error);
        }
      }
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  };

  const shouldTriggerSearch = (filters) => {
    return (
      filters.localidad.trim() !== "" || // Verifica si localidad no está vacía
      filters.codPostal.trim() !== "" || // Verifica si codPostal no está vacío
      filters.provincia.trim() !== "" || // Verifica si provincia no está vacía
      filters.tipo !== "all"             // Verifica si tipo no es el valor por defecto
    );
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

  const TipoMapping = {
    0: "Yacimiento arqueologico",
    1: "Iglesia-Ermita",
    2: "Monasterio-Convento",
    3: "Castillo-Fortaleza-Torre",
    4: "Edificio singular",
    5: "Puente",
    6: "Otros",
  };

  function getDescripcion(tipo) {
  return TipoMapping[tipo] || "Descripción no encontrada";
  }

  function updatePostalCode(cod){
    if (cod < 10000) {
      return '' + 0 + cod;
    } else {
      return cod;
    }
  }

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
              <option value="Yacimiento_arqueologico">Yacimiento arqueologico</option>
              <option value="Iglesia-Ermita">Iglesia</option>
              <option value="Monasterio-Convento">Monasterio</option>
              <option value="Castillo-Fortaleza-Torre">Castillo</option>
              <option value="Edificio_singular">Edificio</option>
              <option value="Puente">Puente</option>
              <option value="Otros">Otros</option>
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
        {results.monumentos.length >= 0 ? (
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
                  <td>{getDescripcion(monumento.tipo)}</td>
                  <td>{monumento.direccion}</td>
                  <td>{getLocalidadNombre(monumento.idLocalidad)}</td>
                  <td>{updatePostalCode(monumento.codigoPostal)}</td>
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
