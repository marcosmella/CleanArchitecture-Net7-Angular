import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { enviroment } from "../../enviroments/enviroment";
import { Vehicle } from "../interfaces/vehicle";

@Injectable({
  providedIn: "root"
})
export class VehicleService {
  private _url: string;

  constructor(private _http: HttpClient) {
    this._url = `${enviroment.apiEndPoint}api/Vehicle`;
  }

  public getById(idVehicle: number): Observable<Vehicle> {
    return this._http.get<Vehicle>(`${this._url}/${idVehicle}`);
  }

  public get(): Observable<Vehicle[]> {
    return this._http.get<Vehicle[]>(`${this._url}`);
  }

  public post(vehicle: Vehicle): Observable<Vehicle> {
    return this._http.post<Vehicle>(`${this._url}`, vehicle);
  }

  public put(vehicle: Vehicle): Observable<Vehicle> {
    return this._http.put<Vehicle>(`${this._url}`, vehicle);
  }

  public delete(id: number): Observable<number> {
    return this._http.delete<number>(`${this._url}/${id}`);
}
}
