import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { IDLForwardToAgent } from 'src/app/shared/models/admin/dlforwardToAgent';

@Component({
  selector: 'app-forwards',
  templateUrl: './forwards.component.html',
  styleUrls: ['./forwards.component.css']
})
export class ForwardsComponent implements OnInit {

  forwardsForm: FormGroup;
  
  data: IDLForwardToAgent[]=[];
  
  constructor(private fb: FormBuilder, private activatedRouter: ActivatedRoute) {
    this.forwardsForm = this.fb.group({
      forwards: this.fb.array([]),
    });
  }

  ngOnInit(): void {
    this.activatedRouter.data.subscribe(response => {
      this.data = response.dlforwarddata;
    })
    this.patchValue2();
  }

 
 //teachers: forwards, batches: category, students: officials

  /** Forwards */
  forwards(): FormArray {
    return this.forwardsForm.get("forwards") as FormArray
  }
 
  newForward(): FormGroup {
    return this.fb.group({
      orderNo: '',
      orderDate: '',
      customerName: '',
      categories: this.fb.array([])
    })
  }
 
 
  addForward() {
    this.forwards().push(this.newForward());
  }
 
 
  removeForward(ti: number) {
    this.forwards().removeAt(ti);
  }
 
 
  /** batches */
 
  categories(ti: number): FormArray {
    return this.forwards().at(ti).get("categories") as FormArray
  }
 
 
  newCategory(): FormGroup {
    return this.fb.group({
      categoryName: '',
      charges: 0,
      officials: this.fb.array([])
    })
  }
 
  addCategory(ti: number) {
    this.categories(ti).push(this.newCategory());
  }
 
  removeCategory(ti: number, bi: number) {
    this.categories(ti).removeAt(ti);
  }
 
  /** students */
 
  officials(ti: number, bi: number): FormArray {
    return this.categories(ti).at(bi).get("officials") as FormArray
  }
 
  newOfficial(): FormGroup {
    return this.fb.group({
      agentName: '',
      customerOfficialId: 0,
      dateTimeForwarded: Date,
      emailIdForwardedTo:'',
    })
  }
 
  addOfficial(ti: number, bi: number) {
    this.officials(ti, bi).push(this.newOfficial());
  }
 
  removeOfficial(ti: number, bi: number, si: number) {
    this.officials(ti, bi).removeAt(si);
  }
 
  onSubmit() {
    console.log(this.forwardsForm.value);
  }
 
  patchValue2() {

    /*
       var data = {
      teachers: [
        {
          name: 'Teacher 1', batches: [
            { name: 'Batch No 1', students: [{ name: 'Ramesh' }, { name: 'Suresh' }, { name: 'Naresh' }] },
            { name: 'Batch No 2', students: [{ name: 'Vikas' }, { name: 'Harish' }, { name: 'Lokesh' }] },
          ]
        },
        {
          name: 'Teacher 2', batches: [
            { name: 'Batch No 3', students: [{ name: 'Ramesh 2' }, { name: 'Suresh 3' }, { name: 'Naresh 4' }] },
            { name: 'Batch No 4', students: [{ name: 'Vikas 3' }, { name: 'Harish 3' }, { name: 'Lokesh 4'  }] },
          ]
        }
      ]
    }
    */
    this.clearFormArray();
    
    this.data.forEach(t => {
      var forward: FormGroup = this.newForward();
      this.forwards().push(forward);
      t.dlForwardCategories.forEach(b => {
        var category = this.newCategory();
   
        (forward.get("categories") as FormArray).push(category)
   
        b.dlForwardCategoryOfficials.forEach(s => {
          (category.get("officials") as FormArray).push(this.newOfficial())
        })
   
      });
    });
   
    this.forwardsForm.patchValue(this.data);
  }
   
   
  clearFormArray() {
   
    this.forwards().clear();
   
  }
}
