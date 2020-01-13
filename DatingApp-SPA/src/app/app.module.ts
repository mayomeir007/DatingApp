import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/Http';
import { AppComponent } from './app.component';
import { ValuesComponent } from './Values/Values.component';

@NgModule({
   declarations: [
      AppComponent,
      ValuesComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule
   ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
