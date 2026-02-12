'use client';

import {Question} from "@/lib/types";
import {Chip} from "@heroui/chip";
import {Avatar} from "@heroui/avatar";
import {LinkComponent} from "@/components/LinkComponent";
import { fuzzyTimeAgo } from "@/lib/utils";


type Props = {
    question: Question;
}
export default function QuestionFooter({question}: Props) {
    return (
        <div className='flex justify-between mt-2'>
            <div className='flex flex-col self-end'>
                <div className="flex gap-2">
                    {question.tagSlugs.map((tag: string) => (
                        <Chip
                            as={LinkComponent}
                            variant='bordered'
                            href={`/questions?tag=${tag}`}
                            key={tag}
                        >
                            {tag}
                        </Chip>
                    ))}
                </div>
            </div>

            <div className='flex flex-col basis-2/5 bg-primary/10 px-3 py-2 gap-2 rounded-lg'>
                <span className='text-sm font-extralight'>asked {fuzzyTimeAgo(question.createdAt)}</span>
                <div className='flex items-center gap-3'>
                    <Avatar className='h-6 w-6' color='secondary'
                            name={question.askerName?.charAt(0)} />
                    <div className='flex flex-col items-center'>
                        <span>{question.askerName}</span>
                        <span className='self-start text-sm font-semibold'>
                            {question.askerName}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
}