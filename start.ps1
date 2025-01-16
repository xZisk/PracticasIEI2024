# Iniciar los proyectos
Start-Process "dotnet" "run --project CargaAPI/CargaAPI.csproj"
Start-Process "dotnet" "run --project BusquedaAPI/BusquedaAPI.csproj"
Start-Process "dotnet" "run --project WrapperXML/WrapperXML.csproj"
Start-Process "dotnet" "run --project WrapperJSON/WrapperJSON.csproj"
Start-Process "dotnet" "run --project WrapperCSV/WrapperCSV.csproj"
Start-Process "npm" "start --prefix Frontend"
