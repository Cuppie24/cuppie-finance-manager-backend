import React, { forwardRef } from "react"
import DatePicker from "react-datepicker"
import "react-datepicker/dist/react-datepicker.css"
import { ru } from "date-fns/locale"
import { Calendar } from "lucide-react"
import { cn } from "@/lib/utils"

interface DateTimePickerProps {
  selected?: Date | null
  onChange: (date: Date | null) => void
  showTimeSelect?: boolean
  dateFormat?: string
  placeholderText?: string
  className?: string
  isClearable?: boolean
  portalId?: string
}

const CustomInput = forwardRef<HTMLButtonElement, any>(({ value, onClick, placeholder, className }, ref) => (
  <button
    type="button"
    onClick={onClick}
    ref={ref}
    className={cn(
      "flex h-10 w-full items-center justify-start rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background",
      "placeholder:text-muted-foreground",
      "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
      "disabled:cursor-not-allowed disabled:opacity-50",
      !value && "text-muted-foreground",
      className
    )}
  >
    <Calendar className="mr-2 h-4 w-4" />
    {value || placeholder}
  </button>
))
CustomInput.displayName = "CustomInput"

export const DateTimePicker: React.FC<DateTimePickerProps> = ({
  selected,
  onChange,
  showTimeSelect = true,
  dateFormat = "dd.MM.yyyy HH:mm",
  placeholderText = "Выберите дату",
  className,
  isClearable = true,
  portalId
}) => {
  return (
    <DatePicker
      selected={selected}
      onChange={onChange}
      showTimeSelect={showTimeSelect}
      timeFormat="HH:mm"
      timeIntervals={15}
      dateFormat={dateFormat}
      locale={ru}
      placeholderText={placeholderText}
      customInput={<CustomInput className={className} />}
      isClearable={isClearable}
      className="w-full"
      withPortal={portalId !== undefined}
      portalId={portalId}
      popperClassName="z-[9999]"
    />
  )
}

export default DateTimePicker
