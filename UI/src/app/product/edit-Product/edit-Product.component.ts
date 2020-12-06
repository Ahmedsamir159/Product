import { Component, OnInit , Inject, ViewChild, ElementRef} from '@angular/core';
import {Router} from "@angular/router";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {first} from "rxjs/operators";
import {Product} from "../../model/Product.model";
import {ApiService} from "../../service/api.service";
import { $ } from 'protractor';

@Component({
  selector: 'app-edit-Product',
  templateUrl: './edit-Product.component.html',
  styleUrls: ['./edit-Product.component.css']
})
export class EditProductComponent implements OnInit {
  @ViewChild('myInput', {static: true}) myInput: ElementRef;
  imageUrl: string = "/assets/img/default-image.png";
  product: Product;
  editForm: FormGroup;
  fileToUpload: File = null;

  constructor(private formBuilder: FormBuilder,private router: Router, private apiService: ApiService) { }

  ngOnInit() {
    this.imageUrl="http://localhost:60310/image/mmmmmmmmmm205219781.jpg"

    let productId = window.localStorage.getItem("editproductId");
    if(!productId) {
      alert("Invalid action.")
      this.router.navigate(['list-product']);
      return;
    }
    this.editForm = this.formBuilder.group({
      Id: [''],
      Name: ['', Validators.required],
      Price: ['', Validators.required],

      Photo: ['', Validators.required],
    });
    this.apiService.getproductById(+productId)
      .subscribe( data => {
        this.imageUrl="http://localhost:60310/image/"+data["Photo"]
        this.editForm.setValue(data);
      });
  }
  handleFileInput(file: FileList) {
    this.fileToUpload = file.item(0);

    //Show image preview
    var reader = new FileReader();
    reader.onload = (event:any) => {
      this.imageUrl = event.target.result;
    }
    reader.readAsDataURL(this.fileToUpload);
  }

  onSubmit() {

    this.apiService.updateproduct(this.editForm.value,this.fileToUpload)
      .subscribe(
        data => {
            alert('Product updated successfully.');
            this.router.navigate(['list-product']);       
        },
        error => {
          alert(JSON.stringify( error));
        });
  }

}
