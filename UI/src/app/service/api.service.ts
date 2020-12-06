import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Product} from "../model/Product.model";
import {Observable} from "rxjs/index";
import {ApiResponse} from "../model/api.response";

@Injectable()
export class ApiService {

  constructor(private http: HttpClient) { }
  baseUrl: string = 'http://localhost:60310/api/product';

  // login(loginPayload) : Observable<ApiResponse> {
  //   return this.http.post<ApiResponse>('http://localhost:8080/' + 'token/generate-token', loginPayload);
  // }

  getproducts() : Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.baseUrl);
  }
  Seacrh(key:String) : Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.baseUrl+"/GetProductsByQuery?query="+key);
  }
  Downloadwithoutquery() : Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.baseUrl+"/download");
  }
  Download(key:String) : Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this.baseUrl+"/download?query="+key);
  }
  getproductById(id: number): Observable<ApiResponse> {
    return this.http.get<any>(this.baseUrl +"/"+ id);
  }
  exportData(id: string): Observable<ApiResponse> {
    return this.http.get<any>(this.baseUrl +"/export"+ id);
  }

  createproduct(Product: Product,fileToUpload: File): Observable<ApiResponse> {
    const fromdata:FormData=new FormData();
    fromdata.append("product",JSON.stringify(Product))
    fromdata.append('Image', fileToUpload);
    return this.http.post<any>(this.baseUrl, fromdata);
  }

  updateproduct(Product: Product,fileToUpload: File): Observable<any> {
    const fromdata:FormData=new FormData();
    let productId = window.localStorage.getItem("editproductId");
    fromdata.append("product",JSON.stringify(Product))
    fromdata.append('Image', fileToUpload);
    return this.http.put<any>(this.baseUrl +"/"+ productId, fromdata);
  }

  deleteproduct(id: number): Observable<ApiResponse> {
    return this.http.delete<any>(this.baseUrl + "/"+id);
  }
}
