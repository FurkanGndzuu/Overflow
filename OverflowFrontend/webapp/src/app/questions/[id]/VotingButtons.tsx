'use client';

import {Button} from "@heroui/button";
import {ArrowDownCircleIcon, ArrowUpCircleIcon} from "@heroicons/react/24/outline";
import {CheckCircleIcon} from "@heroicons/react/24/solid";


type Params = {
    accepted? : boolean;
}


export default function VotingButtons({accepted}: Params) {
   
    
    return (
       <div className="shrink-0 flex flex-col gap-3 items-center justify-start mt-4">
           <Button
               isIconOnly
               variant='light'
           >
               <ArrowUpCircleIcon className='w-12' />
           </Button>
           <span className='text-xl font-semibold'>0</span>
           <Button
               isIconOnly
               variant='light'
           >
               <ArrowDownCircleIcon className='w-12' />
           </Button>

          {accepted && <CheckCircleIcon className='w-12 text-success' />}
       </div>
    );
}