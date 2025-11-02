import React, { useState } from "react"
import { format } from "date-fns"
import { ru } from "date-fns/locale"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import { ChevronDown, X, Calendar as CalendarIcon } from "lucide-react"
import { cn } from "@/lib/utils"
import { Checkbox } from "@/components/ui/checkbox"
import { Separator } from "@/components/ui/separator"

interface TransactionFilters {
  type?: "all" | "income" | "expense"
  categories?: number[]
  fromDate?: Date
  toDate?: Date
}

interface TransactionFiltersProps {
  filters: TransactionFilters
  allCategories: { id: number; name: string }[]
  onChange: (newFilters: TransactionFilters) => void
}

const TransactionFilter: React.FC<TransactionFiltersProps> = ({ filters, allCategories, onChange }) => {
  const [showCategories, setShowCategories] = useState(false)
  const [fromDateOpen, setFromDateOpen] = useState(false)
  const [toDateOpen, setToDateOpen] = useState(false)

  const handleTypeChange = (value: string) => {
    const val = value as "all" | "income" | "expense"
    onChange({ ...filters, type: val === "all" ? undefined : val })
  }

  const handleCategoryToggle = (categoryId: number) => {
    const current = filters.categories ?? []
    const newCategories = current.includes(categoryId)
      ? current.filter((id) => id !== categoryId)
      : [...current, categoryId]
    onChange({ ...filters, categories: newCategories })
  }

  const handleDateChange = (field: "fromDate" | "toDate", date: Date | undefined) => {
    onChange({ ...filters, [field]: date })
  }

  const handleReset = () => {
    onChange({})
  }

  return (
    <div className="flex flex-wrap gap-4 p-4 bg-white rounded-xl shadow-sm mb-6 items-end border border-slate-200">
      {/* Тип операции */}
      <div className="flex flex-col">
        <Label className="mb-2">Тип</Label>
        <Select
          value={filters.type ?? "all"}
          onValueChange={handleTypeChange}
        >
          <SelectTrigger className="w-[140px]">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Все</SelectItem>
            <SelectItem value="income">Доход</SelectItem>
            <SelectItem value="expense">Расход</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Категории */}
      <div className="flex flex-col">
        <Label className="mb-2">Категории</Label>
        <Popover open={showCategories} onOpenChange={setShowCategories}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className="w-[200px] justify-between"
            >
              {filters.categories && filters.categories.length > 0
                ? `${filters.categories.length} выбрано`
                : "Выберите категории"}
              <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-[200px] p-0">
            <div className="p-2 max-h-64 overflow-y-auto">
              {allCategories.map((category) => (
                <label
                  key={category.id}
                  className="flex items-center gap-2 py-2 px-2 rounded hover:bg-accent cursor-pointer"
                >
                  <Checkbox
                    checked={filters.categories?.includes(category.id) ?? false}
                    onCheckedChange={() => handleCategoryToggle(category.id)}
                  />
                  <span className="text-sm">{category.name}</span>
                </label>
              ))}
              {filters.categories && filters.categories.length > 0 && (
                <>
                  <Separator className="my-2" />
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => {
                      onChange({ ...filters, categories: [] })
                    }}
                    className="w-full justify-center text-xs"
                  >
                    <X className="w-3 h-3 mr-1" />
                    Очистить
                  </Button>
                </>
              )}
            </div>
          </PopoverContent>
        </Popover>
      </div>

      {/* Даты */}
      <div className="flex flex-col">
        <Label className="mb-2">Период</Label>
        <div className="flex items-center gap-2">
          <Popover open={fromDateOpen} onOpenChange={setFromDateOpen}>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "w-[140px] justify-start text-left font-normal",
                  !filters.fromDate && "text-muted-foreground"
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {filters.fromDate ? format(filters.fromDate, "dd.MM.yyyy", { locale: ru }) : "От"}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={filters.fromDate}
                onSelect={(date) => {
                  handleDateChange("fromDate", date)
                  setFromDateOpen(false)
                }}
                initialFocus
                locale={ru}
              />
            </PopoverContent>
          </Popover>
          <span className="text-slate-500">–</span>
          <Popover open={toDateOpen} onOpenChange={setToDateOpen}>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "w-[140px] justify-start text-left font-normal",
                  !filters.toDate && "text-muted-foreground"
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {filters.toDate ? format(filters.toDate, "dd.MM.yyyy", { locale: ru }) : "До"}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={filters.toDate}
                onSelect={(date) => {
                  handleDateChange("toDate", date)
                  setToDateOpen(false)
                }}
                initialFocus
                locale={ru}
              />
            </PopoverContent>
          </Popover>
        </div>
      </div>

      {/* Сброс */}
      <Button
        type="button"
        onClick={handleReset}
        variant="outline"
      >
        Сбросить
      </Button>
    </div>
  )
}

export default TransactionFilter
