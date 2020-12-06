import { RouterModule, Routes } from '@angular/router';
import {AddProductComponent} from "./Product/add-Product/add-Product.component";
import {ListProductComponent} from "./Product/list-Product/list-Product.component";
import {EditProductComponent} from "./Product/edit-Product/edit-Product.component";

const routes: Routes = [
  { path: 'add-product', component: AddProductComponent },
  { path: 'list-product', component: ListProductComponent },
  { path: 'edit-product', component: EditProductComponent },
  {path : '', component : ListProductComponent}
];

export const routing = RouterModule.forRoot(routes);
