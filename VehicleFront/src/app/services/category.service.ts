import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { enviroment } from "../../enviroments/enviroment";
import { Category } from "../interfaces/category";

@Injectable({
  providedIn: "root"
})
export class CategoryService {
  private _url: string;

  constructor(private _http: HttpClient) {
    this._url = `${enviroment.apiEndPoint}api/Category`;
  }

  public getById(idCategory: number): Observable<Category> {
    return this._http.get<Category>(`${this._url}/${idCategory}`);
  }

  public get(): Observable<Category[]> {
    return this._http.get<Category[]>(`${this._url}`);
  }

  public post(category: Category): Observable<Category> {
    return this._http.post<Category>(`${this._url}`, category);
  }

  public put(category: Category): Observable<Category> {
    return this._http.put<Category>(`${this._url}`, category);
  }

  public delete(id: number): Observable<number> {
    return this._http.delete<number>(`${this._url}/${id}`);
  }
}
