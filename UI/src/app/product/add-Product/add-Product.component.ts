import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {ApiService} from "../../service/api.service";

@Component({
  selector: 'app-add-Product',
  templateUrl: './add-Product.component.html',
  styleUrls: ['./add-Product.component.css']
})
export class AddProductComponent implements OnInit {
  fileToUpload: File = null;
  imageUrl: string = "/assets/img/default-image.png";

  constructor(private formBuilder: FormBuilder,private router: Router, private apiService: ApiService) { }

  addForm: FormGroup;
  handleFileInput(file: FileList) {
    this.fileToUpload = file.item(0);

    //Show image preview
    var reader = new FileReader();
    reader.onload = (event:any) => {
      this.imageUrl = event.target.result;
    }
    reader.readAsDataURL(this.fileToUpload);
  }
  ngOnInit() {
    this.addForm = this.formBuilder.group({
      id: [],
      Photo: ['', Validators.required],
      Name: ['', Validators.required],
      Price: ['', Validators.required],
    
    });

  }

  onSubmit() {
    this.apiService.createproduct(this.addForm.value,this.fileToUpload)
      .subscribe( data => {
        this.router.navigate(['list-product']);
      });
  }

}
