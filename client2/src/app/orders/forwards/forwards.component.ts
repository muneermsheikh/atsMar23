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
      dlforward: this.fb.array([]),
    });
  }

  ngOnInit(): void {
    this.activatedRouter.data.subscribe(response => {
      this.data = response.dlforwarddata;
      
    })
    console.log('dlforward:', this.data);
    this.patchValue2();
  }

 
 //teachers: forwards, batches: category, students: officials
  /** Forwards */
  dlforward(): FormArray {
    return this.forwardsForm.get("dlforward") as FormArray
  }

  newForward(): FormGroup {
    return this.fb.group({
      orderNo: '',
      orderDate: '',
      customerName: '',
      dlForwardCategories: this.fb.array([])
    })
  }
 
 
  addForward() {
    this.dlforward().push(this.newForward());
  }
 
 
  removeForward(ti: number) {
    this.dlforward().removeAt(ti);
  }
 
 
  /** batches */
 
  dlForwardCategories(ti: number): FormArray {
    return this.dlforward().at(ti).get("dlForwardCategories") as FormArray
  }
 
 
  newCategory(): FormGroup {
    return this.fb.group({
      categoryName: '',
      charges: 0,
      dlForwardCategoryOfficials: this.fb.array([])
    })
  }
 
  addCategory(ti: number) {
    this.dlForwardCategories(ti).push(this.newCategory());
  }
 
  removeCategory(ti: number, bi: number) {
    this.dlForwardCategories(ti).removeAt(ti);
  }
 
  /** students */
 
  dlForwardCategoryOfficials(ti: number, bi: number): FormArray {
    return this.dlForwardCategories(ti).at(bi).get("dlForwardCategoryOfficials") as FormArray
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
    this.dlForwardCategoryOfficials(ti, bi).push(this.newOfficial());
  }
 
  removeOfficial(ti: number, bi: number, si: number) {
    this.dlForwardCategoryOfficials(ti, bi).removeAt(si);
  }
 
  onSubmit() {
    console.log(this.forwardsForm.value);
  }
 
  patchValue2() {

    /*
    DLForwardToAgents
    DLForwardcategories
    DLForwardCateoryOfficial
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
    console.log('tis.data:', this.data);
    this.data.forEach(t => {
      var forward: FormGroup = this.newForward();
      console.log('t', t);
      console.log('forward', forward);

      this.dlforward().push(forward);
      t.dlForwardCategories.forEach(b => {
        var category = this.newCategory();
   
        (forward.get("dlForwardCategories") as FormArray).push(category)
   
        b.dlForwardCategoryOfficials.forEach(s => {
          (category.get("officials") as FormArray).push(this.newOfficial())
        })
   
      });
    });
   console.log('forwards:', this.dlforward);
    this.forwardsForm.patchValue(this.data);
  }
   
   
  clearFormArray() {
   
    this.dlforward().clear();
   
  }
}
