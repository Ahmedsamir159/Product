import { Component, OnInit , Inject, ViewChild, ElementRef} from '@angular/core';
import {Router} from "@angular/router";
import {Product} from "../../model/Product.model";
import {ApiService} from "../../service/api.service";

@Component({
  selector: 'app-list-Product',
  templateUrl: './list-Product.component.html',
  styleUrls: ['./list-Product.component.css']
})
export class ListProductComponent implements OnInit {

  products: any;
  @ViewChild('myInput', {static: true}) myInput: ElementRef;

  constructor(private router: Router, private apiService: ApiService,) { }

  ngOnInit() {
    // if(!window.localStorage.getItem('token')) {
    //   this.router.navigate(['login']);
    //   return;
    // }
    this.apiService.getproducts()
      .subscribe( data => {
        this.products = data;

      });
  }

  deleteProduct(Product: Product): void {
    this.apiService.deleteproduct(Product["Id"])
      .subscribe( data => {
        this.products = this.products.filter(u => u !== Product);
      })
  };

  search():void{
    
    let inputValue = this.myInput.nativeElement.value;
    if( inputValue!="")
    {
  this.apiService.Seacrh(inputValue)
    .subscribe( data => {
      this.products = data;

    });
  }
    else
    {
      this.apiService.getproducts()
      .subscribe( data => {
        this.products = data;

      });
    }

}
  editProduct(Product: Product): void {
    window.localStorage.removeItem("editproductId");
    window.localStorage.setItem("editproductId", Product["Id"].toString());
    this.router.navigate(['edit-product']);
  };

  addProduct(): void {
    this.router.navigate(['add-product']);
  };
  export():void{
  
    this.apiService.Downloadwithoutquery()
    .subscribe( data => {
      const link = document.createElement('a');
      link.setAttribute('target', '_blank');
  
      link.href = "http://localhost:60310/File/"+data+".xlsx";
      link.setAttribute('download', `products.csv`);

      link.click();
      link.remove();

    });
  }
}
